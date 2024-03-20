using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.InstitutionDTOs;

namespace UserManagement.DTOs.HealthCenterDTOs
{
    public class HealthCenterDTO : InstitutionDTO
    {
        public string[] DoctorsId { get; set; } = Array.Empty<string>();
    }
}