using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.InstitutionDTOs;

namespace UserManagement.DTOs.PharmacyDTOs
{
    public class PharmacyDTO : InstitutionDTO
    {
        public required string WorkingHours { get; set; }
        public string HealthCenterName { get; set; } = string.Empty;
    }
}