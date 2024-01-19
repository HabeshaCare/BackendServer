using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
            if (status == 1 && doctor != null)
            {
                return (status, message, _mapper.Map<UsageDoctorDTO>(doctor));
            }

            return (status, message, null);
        }

        public async Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId)
        {
            var filter = Builders<Doctor>.Filter.And(
                Builders<Doctor>.Filter.Eq("Role", UserRole.Doctor),
                Builders<Doctor>.Filter.Eq("_id", ObjectId.Parse(doctorId))
            );

            var update = Builders<Doctor>.Update.Set(d => d.Verified, true);
            var options = new FindOneAndUpdateOptions<Doctor>
            {
                ReturnDocument = ReturnDocument.After,
            };
            try
            {
                var result = await _collection.FindOneAndUpdateAsync(filter, update, options);
                if (result != null)
                    return (1, "Doctor Verified", _mapper.Map<UsageDoctorDTO>(result));
                else
                    return (0, "Doctor not found", null);
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

                    doctor.Verified = false;
                    var result = await _collection.FindOneAndReplaceAsync(filter, doctor, options);


                    UsageDoctorDTO updatedDoctorDTO = _mapper.Map<UsageDoctorDTO>(result);

                    if (result == null)
                    {
                        return (0, "Error updating Doctor", null);
                    }

                    return (1, "Doctor profile updated successfully. Status set to unverified until approved by Admin", updatedDoctorDTO);
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
                var results = await _collection.Find(d => d.Verified == true && d.Role == UserRole.Doctor)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                UsageDoctorDTO[] doctors = _mapper.Map<UsageDoctorDTO[]>(results);
                return (1, null, doctors);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string?, UsageDoctorDTO[])> GetDoctors(DoctorFilterDTO filterOptions, int page, int size)
        {
            var filterBuilder = Builders<Doctor>.Filter;
            var filterDefinition = filterBuilder.Empty;

            filterDefinition &= filterBuilder.Eq("Verified", true);
            filterDefinition &= filterBuilder.Eq("Role", UserRole.Doctor);

            int skip = (page - 1) * size;

            if (filterOptions.MinYearExperience.HasValue)
                filterDefinition &= filterBuilder.Gte("YearOfExperience", filterOptions.MinYearExperience);

            if (filterOptions.MaxYearExperience.HasValue)
                filterDefinition &= filterBuilder.Lte("YearOfExperience", filterOptions.MaxYearExperience);

            if (!string.IsNullOrEmpty(filterOptions.Specialization))
                filterDefinition &= filterBuilder.Eq("Specialization", filterOptions.Specialization);
            try
            {
                var foundDoctors = await _collection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundDoctors.Count == 0)
                    return (0, "No matching doctors found", Array.Empty<UsageDoctorDTO>());

                UsageDoctorDTO[] doctors = _mapper.Map<UsageDoctorDTO[]>(foundDoctors);
                return (1, $"Found {foundDoctors.Count} matching doctors", doctors);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }
    }
}