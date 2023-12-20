using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Models.DTOs.UserDTOs
{
    public class UpdateDTO : RegistrationDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Profession { get; set; }

        public string? City { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }
        public string? ImageUrl { get; set; }

        public override void MapFromUser(User user)
        {
            base.MapFromUser(user);
            Id = user.Id;
            Profession = user.Profession;
            City = user.City;
            Age = user.Age;
            ImageUrl = user.ImageUrl;
        }
    }
}