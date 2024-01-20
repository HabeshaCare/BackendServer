using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UpdateDoctorDTO : UpdateUserDTO
    {
        public String? Specialization { get; set; }
        public int? YearOfExperience { get; set; }
    }
}