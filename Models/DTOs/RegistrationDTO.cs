using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UserAuthentication.Exceptions;

namespace UserAuthentication.Models.DTOs
{
    public class RegistrationDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phonenumber { get; set; }

        public string? Id { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        public string Profession { get; set; }


        [Range(1, 150)]

        [Required(ErrorMessage = "This is a required field")]

        private string _password;

        public string Password {get; set;}

        private string _confirmPassword;
        public string ConfirmPassword 
        {
            get => _confirmPassword;
            set
            {
                if (value != this.Password)
                    throw new PasswordMisMatchException();  
                _confirmPassword = value;
            }
        }


    }
}