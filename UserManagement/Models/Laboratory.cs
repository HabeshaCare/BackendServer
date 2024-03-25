using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class Laboratory : Institution
    {
        public List<LabTest> AvailableTests { get; set; } = new();
        public List<string> TestRequestIds { get; set; } = new();
    }
}