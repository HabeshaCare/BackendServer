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
        public LabTest[] AvailableTests { get; set; } = Array.Empty<LabTest>();
        public string[] TestRequestIds { get; set; } = Array.Empty<string>();
        public string HealthCenterName { get; set; } = string.Empty;
    }
}