using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserManagement.Models.DTOs;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UpdateUserDTO : UserDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public new string? Email { get; set; }
        public string? Profession { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }
        public string? Fullname { get; set; }
        public string? Location { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string? ImageUrl { get; set; }
    }
}