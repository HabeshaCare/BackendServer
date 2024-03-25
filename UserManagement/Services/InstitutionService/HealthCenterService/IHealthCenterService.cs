using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.InstitutionService.HealthCenterService
{
    public interface IHealthCenterService : IInstitutionService
    {
        Task<SResponseDTO<List<HealthCenterDTO>>> GetHealthCenters(FilterDTO? filterDefinition, int page, int size);
        Task<SResponseDTO<HealthCenterDTO>> GetHealthCenter(string id);
        Task<SResponseDTO<HealthCenterDTO>> GetHealthCenterByName(string name);
        Task<SResponseDTO<HealthCenterDTO>> AddHealthCenter(HealthCenterDTO healthCenter, string adminId);
        Task<SResponseDTO<HealthCenterDTO>> UpdateHealthCenter(UpdateHealthCenterDTO healthCenterDTO, string healthCenterId);
        Task<SResponseDTO<bool>> SharePatient(string healthCenterId, string patientId, TimeSpan duration);
        Task<SResponseDTO<bool>> ReferPatient(ReferralDTO referralDTO, TimeSpan duration);
        Task<SResponseDTO<List<UsagePatientDTO>>> GetSharedPatients(string healthCenterId);

    }
}