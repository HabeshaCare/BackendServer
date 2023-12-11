using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserAuthentication.Models
{
    public class User
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email {get; set;}

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phonenumber {get; set;}

        public string? Id { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        public string Profession { get; set; }

        public string? City {get; set;}

        [Range(1, 150)]
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "This is a required field")]

        private string _password;
        public string Password 
        { get => _password;  
          set 
          {
            var passwordHasher = new PasswordHasher<User>();
            _password = passwordHasher.HashPassword(this, value);
          }
        }

        public User(string Email, string Phonenumber, string Profession, string Password)
        {
            this.Email = Email;
            this.Phonenumber = Phonenumber;
            this.Profession = Profession;
            this.Password = Password;
        }
    }
}