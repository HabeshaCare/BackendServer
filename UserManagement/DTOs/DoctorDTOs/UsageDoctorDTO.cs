using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UsageDoctorDTO : UsageUserDTO
    {
        public string LicensePath { get; set; }
        public String Specialization { get; set; } = "Medical";
        public int YearOfExperience { get; set; }
        public bool Verified { get; set; }
    }
}