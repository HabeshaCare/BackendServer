using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.PatientDTOs
{
    public class ReferralDTO
    {
        public string PatientId { get; set; } = "";
        public string DoctorId { get; set; } = "";
        public string HealthCenterId { get; set; } = "";
    }
}