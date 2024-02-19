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
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phonenumber { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "This is a required field")]
        public string Profession { get; set; }

        public string Fullname { get; set; } = "";

        public string? Gender { get; set; }

        public string? City { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }

        public string? ImageUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Normal;

        [Required(ErrorMessage = "This is a required field")]
        public string Password { get; set; } = "";
    }
}