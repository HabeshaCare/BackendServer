using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }
}