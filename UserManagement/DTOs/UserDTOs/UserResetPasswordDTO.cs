using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.DTOs.UserDTOs
{
    public class UserResetPasswordDTO
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}