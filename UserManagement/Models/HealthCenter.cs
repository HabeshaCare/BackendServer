using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class HealthCenter : Institution
    {
        public List<string> DoctorsId { get; set; } = new();
        public List<SharedPatient> SharedPatients { get; set; } = new();
    }
}