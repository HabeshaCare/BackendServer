using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class HealthCenter : Institution
    {
        public string[] DoctorsId { get; set; } = Array.Empty<string>();
        public SharedPatient[] SharedPatients { get; set; } = Array.Empty<SharedPatient>();
    }
}