using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.InstitutionDTOs
{
    public class UpdateInstitutionDTO
    {
        public string? Name { get; set; } = null;
        public string? Location { get; set; } = null;
        public string? LicensePath { get; set; } = null;
        public bool? Verified { get; set; } = null;
    }
}