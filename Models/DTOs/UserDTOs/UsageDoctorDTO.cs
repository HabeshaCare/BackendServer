using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UsageDoctorDTO : UsageUserDTO
    {
        public String Specialization { get; set; } = "Medical";
        public int YearOfExperience { get; set; }
        public bool Verified {get; set;}
    }
}