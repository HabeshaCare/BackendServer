using System.ComponentModel.DataAnnotations;
using UserAuthentication.Models.DTOs;

namespace Backend.Services.User
{
    public class UpdateDTO : RegistrationDTO
    {
        public string Profession { get; set; }

        public string? City { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }
    }
}