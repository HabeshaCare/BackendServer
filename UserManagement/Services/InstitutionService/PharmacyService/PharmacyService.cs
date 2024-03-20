using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService.HealthCenterService;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService
{
    public class PharmacyService : InstitutionService<Pharmacy>, IPharmacyService
    {
        private readonly IHealthCenterService _healthCenterService;
        public PharmacyService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IHealthCenterService healthCenterService) : base(options, fileService, mapper)
        {
            _healthCenterService = healthCenterService;
        }

        public async Task<(int, string, PharmacyDTO?)> AddPharmacy(PharmacyDTO pharmacyDTO)
        {
            string healthCenterId = await HealthCenterExists(pharmacyDTO.HealthCenterName);
            bool healthCenterAddedButNotFound = pharmacyDTO.HealthCenterName != string.Empty && healthCenterId == string.Empty;

            // We don't care for the healthCenter existence if there is none given in the case of pharmacy
            if (healthCenterAddedButNotFound)
                return (0, "Health Center not found. Make sure you're sending an existing health center's name", null);

            Pharmacy pharmacy = _mapper.Map<Pharmacy>(pharmacyDTO);
            pharmacy.Type = InstitutionType.Pharmacy;
            pharmacy.HealthCenterId = healthCenterId;

            return await AddInstitution<PharmacyDTO>(pharmacy);
        }

        public async Task<(int, string?, PharmacyDTO[])> GetPharmacies(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<PharmacyDTO>(filterOption!, page, size);

        }

        public async Task<(int, string?, Pharmacy?)> GetPharmacy(string id)
        {
            return await GetInstitutionById<Pharmacy>(id);
        }

        public async Task<(int, string, PharmacyDTO?)> UpdatePharmacy(PharmacyDTO pharmacyDTO, string pharmacyId)
        {
            string healthCenterId = "";

            // We don't care for the healthCenter existence if there is none given in the case of pharmacy
            if (pharmacyDTO.HealthCenterName != string.Empty)
            {
                healthCenterId = await HealthCenterExists(pharmacyDTO.HealthCenterName);

                if (healthCenterId == string.Empty)
                    return (0, "Health Center not found. Make sure you're sending an existing health center's name", null);
            }

            var (status, message, pharmacy) = await UpdateInstitution<PharmacyDTO, PharmacyDTO>(pharmacyDTO, pharmacyId, healthCenterId);
            if (status == 1 && pharmacy != null)
                pharmacy.HealthCenterName = pharmacyDTO.HealthCenterName;

            return (status, message, pharmacy);
        }

        private async Task<string> HealthCenterExists(string healthCenterName)
        {
            //Check if the health center exists

            var (_, _, healthCenter) = await _healthCenterService.GetHealthCenterByName(healthCenterName);
            string healthCenterId = healthCenter?.Id ?? string.Empty;

            return healthCenterId;
        }
    }
}