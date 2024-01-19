using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UsageUserDTO : UserDTO
    {
        public string Id { get; set; }
        public string Profession { get; set; }
        public string Phonenumber { get; set; }
        public string? City { get; set; }
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }


        public UsageUserDTO()
        {
        }

        public UsageUserDTO(User user)
        {
            MapFromUser(user);
        }
        public override void MapFromUser(User user)
        {
            Email = user.Email;
            Id = user.Id!;
            Role = user.Role;
            Profession = user.Profession;
            Phonenumber = user.Phonenumber;
            City = user.City;
            Age = user.Age;
            ImageUrl = user.ImageUrl;
        }
    }
}