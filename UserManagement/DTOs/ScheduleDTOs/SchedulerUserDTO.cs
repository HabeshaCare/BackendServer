using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.ScheduleDTOs
{
    public class SchedulerUserDTO
    {
        public string Id { get; set; } = "";
        public string Fullname { get; set; } = "";
        public string Gender { get; set; }
        public string Phonenumber { get; set; } = "";
        public string? City { get; set; }
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }
    }
}