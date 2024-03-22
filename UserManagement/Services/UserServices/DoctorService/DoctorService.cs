using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class DoctorService : UserService<Doctor>, IDoctorService
    {
        public DoctorService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        private async Task<SResponseDTO<Doctor>> GetDoctor(string doctorId)
        {
            return await GetUser(doctorId);
        }

        public async Task<SResponseDTO<UsageDoctorDTO>> GetDoctorById(string doctorId)
        {
            return await GetUserById<UsageDoctorDTO>(doctorId);
        }

        public async Task<SResponseDTO<UsageDoctorDTO[]>> GetDoctors(FilterDTO filterOptions, int page, int size)
        {
            var filterDefinition = PrepareFilterDefinition(filterOptions);

            return await GetUsers<UsageDoctorDTO>(filterDefinition, page, size);
        }

        public async Task<SResponseDTO<UsageDoctorDTO>> UpdateDoctor(UpdateDoctorDTO doctorDTO, string doctorId)
        {
            try
            {
                var response = await GetDoctor(doctorId);
                if (response.Success)
                {
                    Doctor doctor = response.Data!;
                    bool changedCriticalInformation =
                        doctorDTO.Fullname != doctor.Fullname ||
                        doctorDTO.Gender != doctor.Gender ||
                        doctorDTO.LicensePath != doctor.LicensePath ||
                        doctorDTO.Specialization != doctor.Specialization ||
                        doctorDTO.YearOfExperience != doctor.YearOfExperience;


                    if (changedCriticalInformation)
                        doctorDTO.Verified = false;

                    _mapper.Map(doctorDTO, doctor);

                    var filter = Builders<Doctor>.Filter.And(
                        Builders<Doctor>.Filter.Eq(u => u.Id, doctorId));

                    var updateResponse = await UpdateUser<UpdateDoctorDTO, UsageDoctorDTO>(doctorDTO, doctorId);


                    if (!updateResponse.Success)
                    {
                        return new() { StatusCode = updateResponse.StatusCode, Errors = updateResponse.Errors };
                    }
                    return new() { StatusCode = updateResponse.StatusCode, Message = "Doctor profile updated successfully", Data = updateResponse.Data };
                }
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<Doctor>> AddDoctor(Doctor user)
        {
            return await AddUser<Doctor>(user);
        }

        public async Task<SResponseDTO<UsageDoctorDTO>> VerifyDoctor(string doctorId)
        {
            var filter = Builders<Doctor>.Filter.And(
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
                    return new() { StatusCode = 201, Message = "Doctor Verified", Data = _mapper.Map<UsageDoctorDTO>(result) };
                else
                    return new() { StatusCode = 404, Message = "Doctor not found" };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<UsageDoctorDTO>> UploadLicense(IFormFile licenseInformation, string doctorId)
        {
            try
            {
                string? licenseInformationPath = null;
                var response = await GetDoctor(doctorId);
                Doctor doctor = response.Data!;
                if (response.Success)
                {
                    var fileResponse = await _fileService.UploadFile(licenseInformation, doctorId, "Licenses");
                    if (!fileResponse.Success)
                        return new() { StatusCode = fileResponse.StatusCode, Errors = fileResponse.Errors };

                    licenseInformationPath = fileResponse.Data;
                    doctor.LicensePath = fileResponse.Data;

                    var filter = Builders<Doctor>.Filter.And(
                        Builders<Doctor>.Filter.Eq(u => u.Id, doctorId));

                    var options = new FindOneAndReplaceOptions<Doctor>
                    {
                        ReturnDocument = ReturnDocument.After // or ReturnDocument.Before
                    };

                    doctor.Verified = false;
                    var result = await _collection.FindOneAndReplaceAsync(filter, doctor, options);


                    UsageDoctorDTO updatedDoctorDTO = _mapper.Map<UsageDoctorDTO>(result);

                    if (result == null)
                    {
                        return new() { StatusCode = 500, Errors = new[] { "Error updating Doctor" } };
                    }
                    return new() { StatusCode = 201, Message = "License Information Uploaded Successfully. Status set to unverified until approved by Admin", Success = true };
                }

                return new() { StatusCode = response.StatusCode, Errors = response.Errors };

            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<UsageDoctorDTO>> GetDoctorByEmail(string doctorEmail)
        {
            return await GetUserByEmail<UsageDoctorDTO>(doctorEmail);
        }
    }
}