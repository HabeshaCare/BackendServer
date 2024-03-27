using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.InstitutionDTOs;
using UserManagement.Models;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class LaboratoryDTO : InstitutionDTO
    {
        public List<string> AvailableTests { get; set; } = new();
        public List<string> TestRequestIds { get; set; } = new();
        public string HealthCenterName { get; set; } = string.Empty;
    }
}