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
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }
        public string? Profession { get; set; }
        [Range(1, 150)]
        [Required(ErrorMessage = "This is a required field")]
        private string _password;
        public string Password { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; } = UserRole.Normal;
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (value != Password)
                    throw new PasswordMisMatchException();
                _confirmPassword = value;
            }
        }

        //Patient Specific Information
        public string? NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

        //Doctor Specific Information
        public string? LicensePath { get; set; }
        public string Specialization { get; set; } = "";
        public int? YearOfExperience { get; set; }

        //Admin Specific Information
    }
}