using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public class HealthCenterService : InstitutionService<HealthCenter>, IHealthCenterService
    {
        private readonly IPatientService _patientService;
        public HealthCenterService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IAdminService adminService, IPatientService patientService) : base(options, fileService, mapper, adminService)
        {
            _patientService = patientService;
        }

        public async Task<SResponseDTO<HealthCenterDTO[]>> GetHealthCenters(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<HealthCenterDTO>(filterOption!, page, size);
        }

        public async Task<SResponseDTO<HealthCenterDTO>> GetHealthCenter(string id)
        {
            return await GetInstitutionById<HealthCenterDTO>(id);
        }

        public async Task<SResponseDTO<HealthCenterDTO>> GetHealthCenterByName(string name)
        {
            return await GetInstitutionByName<HealthCenterDTO>(name);
        }

        public async Task<SResponseDTO<HealthCenterDTO>> AddHealthCenter(HealthCenterDTO healthCenter, string adminId)
        {
            HealthCenter _healthCenter = _mapper.Map<HealthCenter>(healthCenter);
            return await AddInstitution<HealthCenterDTO>(_healthCenter, adminId);
        }

        public async Task<SResponseDTO<bool>> SharePatient(string healthCenterId, string patientId, TimeSpan duration)
        {
            //#TODO Configure OTP to ask the patient for confirmation before sharing the information

            try
            {
                var sharedPatient = new SharedPatient { PatientId = patientId, ExpirationTime = DateTime.UtcNow.Add(duration) };
                var filter = Builders<HealthCenter>.Filter.Eq(hc => hc.Id, healthCenterId);
                var update = Builders<HealthCenter>.Update.Push(hc => hc.SharedPatients, sharedPatient);
                await _collection.UpdateOneAsync(filter, update);
                return new() { StatusCode = StatusCodes.Status200OK, Message = "Patient shared successfully", Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Message = ex.Message, Success = false };
            }
        }

        public async Task<SResponseDTO<bool>> ReferPatient(ReferralDTO referralDTO, TimeSpan duration)
        {
            try
            {
                //#TODO: Change this to a more realistic logic later
                return await SharePatient(referralDTO.HealthCenterId, referralDTO.PatientId, duration);
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Message = ex.Message, Success = false };
            }
        }

        public async Task<SResponseDTO<UsagePatientDTO[]>> GetSharedPatients(string healthCenterId)
        {
            try
            {
                var response = await GetHealthCenter(healthCenterId);
                if (!response.Success)
                    return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                var sharedPatients = response.Data!.SharedPatients;
                var patients = Array.Empty<UsagePatientDTO>();
                var tasks = sharedPatients.Select(async sharedPatient =>
                    {
                        var response = await _patientService.GetPatientById(sharedPatient.PatientId ?? string.Empty);

                        if (response.Success)
                            _ = patients.Append(response.Data!);
                    });

                await Task.WhenAll(tasks);

                return new() { StatusCode = StatusCodes.Status200OK, Message = $"Found {patients.Length} shared Patients", Data = patients, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = StatusCodes.Status500InternalServerError, Message = ex.Message, Success = false };
            }
        }

        public async Task<SResponseDTO<HealthCenterDTO>> UpdateHealthCenter(UpdateHealthCenterDTO healthCenterDTO, string healthCenterId)
        {
            return await UpdateInstitution<UpdateHealthCenterDTO, HealthCenterDTO>(healthCenterDTO, healthCenterId);
        }

    }
}