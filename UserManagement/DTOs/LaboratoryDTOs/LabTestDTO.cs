using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.DTOs.LaboratoryDTOs
{
    public class LabTestDTO
    {
        public LabTest[] AvailableTests { get; set; } = Array.Empty<LabTest>();
    }
}