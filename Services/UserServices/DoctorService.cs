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

        public async Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(String doctorId)
        {
            try
            {
                var doctor = await _collection.FindAsync(d => d.Id == doctorId);
                return (1, null, _mapper.Map<UsageDoctorDTO>(doctor));
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, UsageDoctorDTO?)> Update(UpdateDoctorDTO doctorDTO, String doctorId)
        {
            Doctor doctor = _mapper.Map<Doctor>(doctorDTO);
            var filter = Builders<Doctor>.Filter.And(
                Builders<Doctor>.Filter.Eq(u => u.Id, doctorId),
                Builders<Doctor>.Filter.Eq(u => u.Role, UserRole.Admin));

            try
            {
                var result = await _collection.FindOneAndReplaceAsync(filter,doctor);
                UsageDoctorDTO updatedDoctorDTO = _mapper.Map<UsageDoctorDTO>(result);

                if (result != null)
                {
                    return (0, "Doctor Can't be found", null);
                }

                return (1, "Doctor Profile UpdatedSuccessfully", updatedDoctorDTO);
            }
            catch(Exception ex)
            {
                return(0, ex.Message, null);
            }
        }
    }
}