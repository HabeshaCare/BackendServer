using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.DTOs.AdminDTOs
{
    public class UpdateAdminDTO : UpdateUserDTO
    {
        public string InstitutionId { get; set; } = string.Empty;
        public string AssociatedHealthCenterId { get; set; } = string.Empty;
        public bool Verified { get; set; }
    }
}