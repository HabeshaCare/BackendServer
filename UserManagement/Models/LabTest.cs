using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class LabTest
    {
        public string? Id { get; set; }
        public required string TestName { get; set; }
        public string? TestValue { get; set; }

    }
}