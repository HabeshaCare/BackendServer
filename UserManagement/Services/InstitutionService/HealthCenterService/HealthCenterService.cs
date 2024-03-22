using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
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

        public async Task<SResponseDTO<HealthCenterDTO>> AddHealthCenter(HealthCenterDTO healthCenter)
        {
            HealthCenter _healthCenter = _mapper.Map<HealthCenter>(healthCenter);
            return await AddInstitution<HealthCenterDTO>(_healthCenter);
        }

        public async Task<SResponseDTO<HealthCenterDTO>> UpdateHealthCenter(UpdateHealthCenterDTO healthCenterDTO, string healthCenterId)
        {
            return await UpdateInstitution<UpdateHealthCenterDTO, HealthCenterDTO>(healthCenterDTO, healthCenterId);
        }

    }
}