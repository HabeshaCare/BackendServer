using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.Models;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public interface IHealthCenterService : IInstitutionService
    {
        Task<(int, string?, HealthCenterDTO[])> GetHealthCenters(FilterDefinition<HealthCenter> filterDefinition, int page, int size);
        Task<(int, string?, HealthCenter?)> GetHealthCenter(string id);
        Task<(int, string, HealthCenterDTO?)> AddHealthCenter(HealthCenter healthCenter);
        Task<(int, string, HealthCenterDTO?)> UpdateHealthCenter(HealthCenterDTO healthCenterDTO, string healthCenterId);
    }
}