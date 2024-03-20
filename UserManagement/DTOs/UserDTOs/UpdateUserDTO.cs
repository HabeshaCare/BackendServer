using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserManagement.Models.DTOs;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public string? Fullname { get; set; }
        public string? Gender { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? ImageUrl { get; set; }
        public string VerificationToken { get; set; } = string.Empty;
        public DateTime? VerifiedAt { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? ResetTokenExpires { get; set; }
    }
}