using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserManagement.Models.DTOs.UserDTOs
{
    public class UsageUserDTO : UserDTO
    {
        public string Id { get; set; } = "";
        public string Fullname { get; set; } = "";
        public string Phonenumber { get; set; } = "";
        public string? Location { get; set; }
        public DateTime? DateOfBirth { get; set; } = null;
        public string? ImageUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }
}