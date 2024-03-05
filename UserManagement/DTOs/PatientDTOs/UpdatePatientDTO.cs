using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.DTOs.PatientDTOs
{
    public class UpdatePatientDTO : UpdateUserDTO
    {
        public string? NationalId { get; set; } = null;
        public int? Height { get; set; } = null;
        public int? Weight { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
    }
}