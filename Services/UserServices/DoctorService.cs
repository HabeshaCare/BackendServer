using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.OptionsDTO;
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

        private async Task<(int, string?, Doctor?)> GetDoctor(string doctorId)
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
        public async Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(string doctorId)
        {
            var (status, message, doctor) = await GetDoctor(doctorId);
            if(status == 1 && doctor != null)
            {
                return (status, message, _mapper.Map<UsageDoctorDTO>(doctor));
            }

            return (status, message, null);
        }

        public async Task<(int, string, UsageDoctorDTO?)> Update(UpdateDoctorDTO doctorDTO, String doctorId)
        {
            try
            {
                var (status, message, doctor) = await GetDoctor(doctorId);
                if (status == 1 && doctor != null)
                {
                    _mapper.Map(doctorDTO, doctor);

                    var filter = Builders<Doctor>.Filter.And(
                        Builders<Doctor>.Filter.Eq(u => u.Id, doctorId),
                        Builders<Doctor>.Filter.Eq(u => u.Role, UserRole.Doctor));

                    var options = new FindOneAndReplaceOptions<Doctor>
                    {
                        ReturnDocument = ReturnDocument.After // or ReturnDocument.Before
                    };

                    var result = await _collection.FindOneAndReplaceAsync(filter, doctor, options);
                    
                    doctor.Verified = false;

                    UsageDoctorDTO updatedDoctorDTO = _mapper.Map<UsageDoctorDTO>(result);

                    if (result == null)
                    {
                        return (0, "Error updating Doctor", null);
                    }

                    return (1, "Doctor profile updated successfully. Status set to verified until approved by Admin", updatedDoctorDTO);
                }

                return (0, "Doctor not found", null);

            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string?, UsageDoctorDTO[])> GetDoctors(int page, int size)
        {
            
            try
            {
                int skip = (page - 1) * size;
                var results = await _collection.Find(d => d.Verified.GetValueOrDefault() && d.Role == UserRole.Doctor)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();
                
                UsageDoctorDTO[] doctors = _mapper.Map<UsageDoctorDTO[]>(results);
                return (1, null, doctors);
            }
            catch(Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string?, UsageDoctorDTO[])> GetDoctors(int page, int size, DoctorFilterDTO filterOptions)
        {

            throw new NotImplementedException();
        }
    }
}