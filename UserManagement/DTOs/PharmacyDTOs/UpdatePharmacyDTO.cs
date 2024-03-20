using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.InstitutionDTOs;

namespace UserManagement.DTOs.PharmacyDTOs
{
    public class UpdatePharmacyDTO : UpdateInstitutionDTO
    {
        public string WorkingHours { get; set; } = string.Empty;
        public string HealthCenterName { get; set; } = string.Empty;
    }
}