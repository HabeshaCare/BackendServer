using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UserManagement.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Fullname { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Patient;
        [Required(ErrorMessage = "This is a required field")]
        public required string Password { get; set; }
        public string VerificationToken { get; set; } = string.Empty;
        public DateTime? VerificationTokenExpires { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpires { get; set; }
    }
}