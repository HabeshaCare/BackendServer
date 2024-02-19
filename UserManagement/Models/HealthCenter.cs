using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class HealthCenter : Institution
    {
        public string[] Doctors { get; set; } = Array.Empty<string>();
    }
}