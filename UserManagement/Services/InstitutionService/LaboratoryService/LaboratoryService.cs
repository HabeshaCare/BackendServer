using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class LaboratoryService : InstitutionService<Laboratory>, ILaboratoryService
    {
        private readonly IHealthCenterService _healthCenterService;

        public LaboratoryService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IHealthCenterService healthCenterService, IAdminService adminService) : base(options, fileService, mapper, adminService)
        {
            _healthCenterService = healthCenterService;

        }

        public async Task<SResponseDTO<LaboratoryDTO[]>> GetLaboratories(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<LaboratoryDTO>(filterOption!, page, size);
        }

        public async Task<SResponseDTO<Laboratory>> GetLaboratory(string id)
        {
            return await GetInstitutionById<Laboratory>(id);
        }

        public async Task<SResponseDTO<Laboratory>> GetLabTestRequests()
        {

        }

        public async Task<SResponseDTO<LaboratoryDTO>> AddLaboratory(LaboratoryDTO laboratory, string adminId)
        {
            HealthCenterDTO? healthCenter = await HealthCenterExists(laboratory.HealthCenterName);
            string healthCenterId = healthCenter?.Id ?? string.Empty;

            if (laboratory.HealthCenterName == string.Empty || healthCenterId == string.Empty)
                return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };

            Laboratory _laboratory = _mapper.Map<Laboratory>(laboratory);
            _laboratory.Type = InstitutionType.Laboratory;
            _laboratory.AssociatedHealthCenterId = healthCenterId;
            return await AddInstitution<LaboratoryDTO>(_laboratory, adminId);
        }

        public async Task<SResponseDTO<LaboratoryDTO>> UpdateLaboratory(UpdateLaboratoryDTO laboratoryDTO, string laboratoryId)
        {
            HealthCenterDTO? healthCenter;
            string healthCenterId = string.Empty;

            if (laboratoryDTO.HealthCenterName != string.Empty)
            {
                healthCenter = await HealthCenterExists(laboratoryDTO.HealthCenterName);
                healthCenterId = healthCenter?.Id ?? string.Empty;

                if (healthCenterId == string.Empty)
                    return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };
            }

            var response = await UpdateInstitution<UpdateLaboratoryDTO, LaboratoryDTO>(laboratoryDTO, laboratoryId, healthCenterId);
            if (response.Success)
                response.Data!.HealthCenterName = laboratoryDTO.HealthCenterName;

            return new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Success = response.Success, Errors = response.Errors };
        }

        private async Task<HealthCenterDTO?> HealthCenterExists(string healthCenterName)
        {
            //Check if the health center exists

            var response = await _healthCenterService.GetHealthCenterByName(healthCenterName);
            return _mapper.Map<HealthCenterDTO>(response.Data);
        }

        public async Task<SResponseDTO<LaboratoryDTO>> UpdateLabTests(LabTest[] labTests, string laboratoryId)
        {
            var response = await GetLaboratory(laboratoryId);
            if (!response.Success)
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };

            var laboratory = response.Data!;
            laboratory.AvailableTests = labTests;

            return await UpdateInstitution<Laboratory, LaboratoryDTO>(laboratory, laboratoryId);
        }
    }
}