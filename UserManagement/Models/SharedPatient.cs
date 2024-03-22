using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class SharedPatient
    {
        public required string PatientId { get; set; }
        public required DateTime ExpirationTime { get; set; }
    }
}