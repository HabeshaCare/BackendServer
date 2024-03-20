using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class LaboratoryService : InstitutionService<Laboratory>, ILaboratoryService
    {
        private readonly IHealthCenterService _healthCenterService;

        public LaboratoryService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IHealthCenterService healthCenterService) : base(options, fileService, mapper)
        {
            _healthCenterService = healthCenterService;

        }

        public async Task<(int, string?, LaboratoryDTO[])> GetLaboratories(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<LaboratoryDTO>(filterOption!, page, size);
        }

        public async Task<(int, string?, Laboratory?)> GetLaboratory(string id)
        {
            return await GetInstitutionById<Laboratory>(id);
        }

        public async Task<(int, string, LaboratoryDTO?)> AddLaboratory(LaboratoryDTO laboratory)
        {
            HealthCenterDTO? healthCenter = await HealthCenterExists(laboratory.HealthCenterName);
            string healthCenterId = healthCenter?.Id ?? string.Empty;

            if (laboratory.HealthCenterName == string.Empty || healthCenterId == string.Empty)
                return (0, "Health Center not found. Make sure you're sending an existing health center's name", null);

            Laboratory _laboratory = _mapper.Map<Laboratory>(laboratory);
            _laboratory.Type = InstitutionType.Laboratory;
            _laboratory.HealthCenterId = healthCenterId;
            return await AddInstitution<LaboratoryDTO>(_laboratory);
        }

        public async Task<(int, string, LaboratoryDTO?)> UpdateLaboratory(UpdateLaboratoryDTO laboratoryDTO, string laboratoryId)
        {
            HealthCenterDTO? healthCenter;
            string healthCenterId = string.Empty;

            if (laboratoryDTO.HealthCenterName != string.Empty)
            {
                healthCenter = await HealthCenterExists(laboratoryDTO.HealthCenterName);
                healthCenterId = healthCenter?.Id ?? string.Empty;

                if (healthCenterId == string.Empty)
                    return (0, "Health Center not found. Make sure you're sending an existing health center's name", null);
            }

            var (status, message, laboratory) = await UpdateInstitution<UpdateLaboratoryDTO, LaboratoryDTO>(laboratoryDTO, laboratoryId, healthCenterId);
            if (status == 1 && laboratory != null)
                laboratory.HealthCenterName = laboratoryDTO.HealthCenterName;

            return (status, message, laboratory);
        }

        private async Task<HealthCenterDTO?> HealthCenterExists(string healthCenterName)
        {
            //Check if the health center exists

            var (_, _, healthCenter) = await _healthCenterService.GetHealthCenterByName(healthCenterName);
            return _mapper.Map<HealthCenterDTO>(healthCenter);
        }
    }
}