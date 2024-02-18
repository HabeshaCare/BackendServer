using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace BackendServer.Models
{
    public class Patient : User
    {
        public required string NationalId { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public required int Height { get; set; }
        public required int Weight { get; set; }
    }
}