using UserManagement.DTOs.InstitutionDTOs;
using UserManagement.Models;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class UpdateLaboratoryDTO : UpdateInstitutionDTO
    {
        public LabTest[] AvailableTests { get; set; } = Array.Empty<LabTest>();
        public string HealthCenterName { get; set; } = string.Empty;

    }
}