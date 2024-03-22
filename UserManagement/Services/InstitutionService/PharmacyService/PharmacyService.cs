using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class PharmacyService : InstitutionService<Pharmacy>, IPharmacyService
    {
        private readonly IHealthCenterService _healthCenterService;
        public PharmacyService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IHealthCenterService healthCenterService, IAdminService adminService) : base(options, fileService, mapper, adminService)
        {
            _healthCenterService = healthCenterService;
        }

        public async Task<SResponseDTO<PharmacyDTO>> AddPharmacy(PharmacyDTO pharmacyDTO, string adminId)
        {
            HealthCenterDTO? healthCenter = await HealthCenterExists(pharmacyDTO.HealthCenterName);
            string healthCenterId = healthCenter?.Id ?? string.Empty;

            bool healthCenterAddedButNotFound = pharmacyDTO.HealthCenterName != string.Empty && healthCenterId == string.Empty;

            // We don't care for the healthCenter existence if there is none given in the case of pharmacy
            if (healthCenterAddedButNotFound)
                return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };

            Pharmacy pharmacy = _mapper.Map<Pharmacy>(pharmacyDTO);
            pharmacy.Type = InstitutionType.Pharmacy;
            pharmacy.AssociatedHealthCenterId = healthCenterId;

            var response = await AddInstitution<PharmacyDTO>(pharmacy, adminId);

            if (response.Success)
                response.Data!.HealthCenterName = pharmacyDTO.HealthCenterName;

            return new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Success = response.Success, Errors = response.Errors };
        }

        public async Task<SResponseDTO<PharmacyDTO[]>> GetPharmacies(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<PharmacyDTO>(filterOption!, page, size);

        }

        public async Task<SResponseDTO<Pharmacy>> GetPharmacy(string id)
        {
            return await GetInstitutionById<Pharmacy>(id);
        }

        public async Task<SResponseDTO<PharmacyDTO>> UpdatePharmacy(UpdatePharmacyDTO pharmacyDTO, string pharmacyId)
        {
            HealthCenterDTO? healthCenter;
            string healthCenterId = string.Empty;

            // We don't care for the healthCenter existence if there is none given in the case of pharmacy
            if (pharmacyDTO.HealthCenterName != string.Empty)
            {
                healthCenter = await HealthCenterExists(pharmacyDTO.HealthCenterName);
                healthCenterId = healthCenter?.Id ?? string.Empty;

                if (healthCenterId == string.Empty)
                    return new() { StatusCode = 404, Errors = new[] { "Health Center not found. Make sure you're sending an existing health center's name" } };
            }

            var response = await UpdateInstitution<UpdatePharmacyDTO, PharmacyDTO>(pharmacyDTO, pharmacyId, healthCenterId);
            if (response.Success)
                response.Data!.HealthCenterName = pharmacyDTO.HealthCenterName;

            return new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Success = response.Success, Errors = response.Errors };
        }

        private async Task<HealthCenterDTO?> HealthCenterExists(string healthCenterName)
        {
            //Check if the health center exists

            var response = await _healthCenterService.GetHealthCenterByName(healthCenterName);
            return _mapper.Map<HealthCenterDTO>(response.Data);
        }
    }
}