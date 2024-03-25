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
        public List<string> DoctorsId { get; set; } = new();
        public List<SharedPatient> SharedPatients { get; set; } = new();
    }
}