using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class UpdateTestNameDTO
    {
        public string NewTestName { get; set; } = string.Empty;
        public string OldTestName { get; set; } = string.Empty;
    }
}