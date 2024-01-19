using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.DTOs.ScheduleDTOs
{
    public class ScheduleDTO
    {
        public string? Id { get; set; }
        public DateTime ScheduleTime { get; set; }
        public bool Confirmed { get; set; }
        public SchedulerUserDTO? Scheduler { get; set; }
        public ScheduledDoctorDTO? Doctor { get; set; }
    }
}