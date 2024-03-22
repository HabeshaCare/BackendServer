using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UpdateDoctorDTO : UpdateUserDTO
    {
        public string? Location { get; set; }
        public string? LicensePath { get; set; }
        public string? Specialization { get; set; }
        public string AssociatedHealthCenterId { get; set; } = string.Empty;
        public int? YearOfExperience { get; set; }
        public bool Verified { get; set; }
    }
}