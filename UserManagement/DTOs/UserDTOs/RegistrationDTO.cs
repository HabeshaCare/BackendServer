using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserManagement.Exceptions;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Models.DTOs
{
    public class RegistrationDTO : UserDTO
    {
        [Phone]
        public string? Phonenumber { get; set; }
        [Required, MinLength(6)]
        public required string Password { get; set; }
        [Required, Compare("Password")]
        public required string ConfirmPassword { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; } = UserRole.Patient;

        //Patient Specific Information
        public string? NationalId { get; set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public int Height { get; set; }
        public int Weight { get; set; }

        //Doctor Specific Information
        public string? LicensePath { get; set; }
        public string Specialization { get; set; } = "Medical";
        public int? YearOfExperience { get; set; }
    }
}