using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.InstitutionDTOs;
using UserManagement.Models;

namespace UserManagement.DTOs.HealthCenterDTOs
{
    public class HealthCenterDTO : InstitutionDTO
    {
        public string[] DoctorsId { get; set; } = Array.Empty<string>();
        public SharedPatient[] SharedPatients { get; set; } = Array.Empty<SharedPatient>();
    }
}