using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public interface IHealthCenterService : IInstitutionService
    {
        Task<SResponseDTO<HealthCenterDTO[]>> GetHealthCenters(FilterDTO? filterDefinition, int page, int size);
        Task<SResponseDTO<HealthCenterDTO>> GetHealthCenter(string id);
        Task<SResponseDTO<HealthCenterDTO>> GetHealthCenterByName(string name);
        Task<SResponseDTO<HealthCenterDTO>> AddHealthCenter(HealthCenterDTO healthCenter);
        Task<SResponseDTO<HealthCenterDTO>> UpdateHealthCenter(UpdateHealthCenterDTO healthCenterDTO, string healthCenterId);
    }
}