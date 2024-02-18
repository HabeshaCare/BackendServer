using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UpdateDoctorDTO : UpdateUserDTO
    {
        public string? LicensePath { get; set; }
        public String? Specialization { get; set; }
        public int? YearOfExperience { get; set; }
    }
}