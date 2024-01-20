using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UpdateDTO
    {
        [BsonIgnoreIfDefault]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        public string? Profession { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole? Role { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        public string? City { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }

        public IFormFile? Image { get; set; }
    }
}