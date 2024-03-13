using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public class HealthCenterService : InstitutionService<HealthCenter>, IHealthCenterService
    {
        public HealthCenterService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<(int, string?, HealthCenterDTO[])> GetHealthCenters(FilterDTO? filterOption, int page, int size)
        {
            return await GetInstitutions<HealthCenterDTO>(filterOption!, page, size);
        }

        public async Task<(int, string?, HealthCenter?)> GetHealthCenter(string id)
        {
            return await GetInstitutionById<HealthCenter>(id);
        }

        public async Task<(int, string, HealthCenterDTO?)> AddHealthCenter(HealthCenter healthCenter)
        {
            return await AddInstitution<HealthCenterDTO>(healthCenter);
        }

        public async Task<(int, string, HealthCenterDTO?)> UpdateHealthCenter(HealthCenterDTO healthCenterDTO, string healthCenterId)
        {
            return await UpdateInstitution<HealthCenterDTO, HealthCenterDTO>(healthCenterDTO, healthCenterId);
        }

    }
}