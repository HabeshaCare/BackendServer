using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class Pharmacy : Institution
    {
        public required string WorkingHours { get; set; }
    }
}