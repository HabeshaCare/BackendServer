using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.ScheduleDTOs
{
    public class CreateScheduleDTO
    {
        public DateTime ScheduleTime { get; set; }
        public string? DoctorId { get; set; }
    }
}