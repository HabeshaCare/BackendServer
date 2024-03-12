using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
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

        private async Task<(int, string?, Doctor?)> GetDoctor(string doctorId)
        {
            return await GetUser(doctorId);
        }

        public async Task<(int, string?, UsageDoctorDTO?)> GetDoctorById(string doctorId)
        {
            return await GetUserById<UsageDoctorDTO>(doctorId);
        }

        public async Task<(int, string?, UsageDoctorDTO[])> GetDoctors(FilterDTO filterOptions, int page, int size)
        {
            var filterBuilder = Builders<Doctor>.Filter;
            var filterDefinition = filterBuilder.Empty;

            filterDefinition &= filterBuilder.Eq("Verified", true);

            if (filterOptions.MinYearExperience.HasValue)
                filterDefinition &= filterBuilder.Gte("YearOfExperience", filterOptions.MinYearExperience);

            if (filterOptions.MaxYearExperience.HasValue)
                filterDefinition &= filterBuilder.Lte("YearOfExperience", filterOptions.MaxYearExperience);

            if (!string.IsNullOrEmpty(filterOptions.Specialization))
                filterDefinition &= filterBuilder.Eq("Specialization", filterOptions.Specialization);

            return await GetUsers<UsageDoctorDTO>(filterDefinition, page, size);
        }

        public async Task<(int, string, Doctor?)> UpdateDoctor(UpdateDoctorDTO doctorDTO, string doctorId)
        {
            try
            {
                var (status, message, doctor) = await GetDoctor(doctorId);
                if (status == 1 && doctor != null)
                {
                    doctorDTO.Verified = false;
                    _mapper.Map(doctorDTO, doctor);

                    var filter = Builders<Doctor>.Filter.And(
                        Builders<Doctor>.Filter.Eq(u => u.Id, doctorId));

                    var (updateStatus, updateMessage, updatedDoctorDTO) = await UpdateUser<UpdateDoctorDTO, Doctor>(doctorDTO, doctorId);


                    if (updateStatus == 0 || updatedDoctorDTO == null)
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

        public async Task<(int, string, Doctor?)> AddDoctor(Doctor user)
        {
            return await AddUser<Doctor>(user);
        }

        public async Task<(int, string, UsageDoctorDTO?)> VerifyDoctor(string doctorId)
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
                    return (1, "Doctor Verified", _mapper.Map<UsageDoctorDTO>(result));
                else
                    return (0, "Doctor not found", null);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, UsageDoctorDTO?)> UploadLicense(IFormFile licenseInformation, string doctorId)
        {
            try
            {
                string? licenseInformationPath = null;
                var (status, message, doctor) = await GetDoctor(doctorId);
                if (status == 1 && doctor != null)
                {
                    var (fileStatus, fileMessage, filePath) = await _fileService.UploadFile(licenseInformation, doctorId, "Licenses");
                    if (fileStatus == 1 || filePath == null)
                        return (fileStatus, fileMessage, null);

                    licenseInformationPath = filePath;
                    doctor.LicensePath = filePath;

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
                        return (0, "Error updating Doctor", null);
                    }

                    return (1, "License Information Uploaded Successfully. Status set to unverified until approved by Admin", updatedDoctorDTO);
                }

                return (0, "Doctor not found", null);

            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string?, UsageDoctorDTO?)> GetDoctorByEmail(string doctorEmail)
        {
            return await GetUserByEmail<UsageDoctorDTO>(doctorEmail);
        }
    }
}