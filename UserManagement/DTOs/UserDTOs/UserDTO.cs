using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UserDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Gender { get; set; }
    }
}