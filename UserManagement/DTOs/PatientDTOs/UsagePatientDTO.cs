using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.DTOs.PatientDTOs
{
    public class UsagePatientDTO : UsageUserDTO
    {
        public string? NationalId { get; set; } = null;
        public int? Height { get; set; } = null;
        public int? Weight { get; set; } = null;
        public DateTime? DateOfBirth { get; set; }
        public int? Age
        {
            get
            {
                if (DateOfBirth == null)
                    return null;

                DateTime currentDate = DateTime.Now;
                DateTime dateOfBirth = DateOfBirth.Value;

                int age = currentDate.Year - dateOfBirth.Year;

                // Adjust the age if the birthday hasn't occurred yet this year
                if (currentDate.Month < dateOfBirth.Month || (currentDate.Month == dateOfBirth.Month && currentDate.Day < dateOfBirth.Day))
                {
                    age--;
                }

                return age;
            }
        }
    }
}