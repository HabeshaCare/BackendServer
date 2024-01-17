using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.FileServices;
using UserAuthentication.Utils;

namespace UserAuthentication.Services.UserServices
{
    public class DoctorService : MongoDBService, IDoctorService
    {
        private readonly IMongoCollection<Doctor> _collection;
        private readonly IMapper _mapper;
        public DoctorService(IOptions<MongoDBSettings> options, IMapper mapper) : base(options)
        {
            _collection = GetCollection<Doctor>("Users");
            _mapper = mapper;
        }

        private async Task<(int, string?, Doctor?)> GetDoctorById(string doctorId)
        {
            try
            {
                var result = await _collection.FindAsync(d => d.Id == doctorId && d.Role == UserRole.Doctor);
                Doctor doctor = (await result.ToListAsync())[0];
                return (1, null, doctor);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, UsageDoctorDTO?)> Update(UpdateDoctorDTO doctorDTO, String doctorId)
        {
            try
            {
                var (status, message, doctor) = await GetDoctorById(doctorId);
                if (status == 1 && doctor != null)
                {
                    _mapper.Map(doctorDTO, doctor);

                    var filter = Builders<Doctor>.Filter.And(
                        Builders<Doctor>.Filter.Eq(u => u.Id, doctorId),
                        Builders<Doctor>.Filter.Eq(u => u.Role, UserRole.Doctor));

                    var result = await _collection.FindOneAndReplaceAsync(filter, doctor);
                    UsageDoctorDTO updatedDoctorDTO = _mapper.Map<UsageDoctorDTO>(result);

                    if (result == null)
                    {
                        return (0, "Error updating Doctor", null);
                    }

                    return (1, "Doctor profile updated successfully", updatedDoctorDTO);

                }

                return (0, "Doctor not found", null);

            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }
    }
}