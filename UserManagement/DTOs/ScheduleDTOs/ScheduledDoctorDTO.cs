using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.ScheduleDTOs
{
    public class ScheduledDoctorDTO : SchedulerUserDTO
    {
        public string Specialization { get; set; } = "";
        public int YearOfExperience { get; set; }
    }
}