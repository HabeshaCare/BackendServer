using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public interface IHealthCenterService : IInstitutionService
    {
        Task<(int, string?, HealthCenterDTO[])> GetHealthCenters(FilterDTO? filterDefinition, int page, int size);
        Task<(int, string?, HealthCenterDTO?)> GetHealthCenter(string id);
        Task<(int, string?, HealthCenterDTO?)> GetHealthCenterByName(string name);
        Task<(int, string, HealthCenterDTO?)> AddHealthCenter(HealthCenterDTO healthCenter);
        Task<(int, string, HealthCenterDTO?)> UpdateHealthCenter(UpdateHealthCenterDTO healthCenterDTO, string healthCenterId);
    }
}