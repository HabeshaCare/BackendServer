using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }
        public string? Profession { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }
        public string? Fullname { get; set; }
        public char? Gender { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole? Role { get; set; }
        public string? City { get; set; }
        [Range(1, 150)]
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }
    }
}