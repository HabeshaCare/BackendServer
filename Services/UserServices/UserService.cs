using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Utils;

namespace UserAuthentication.Services.UserServices
{
    public class UserService : MongoDBService, IUserService
    {
        private readonly IMongoCollection<User>_collection;
        public UserService(IOptions<MongoDBSettings> options) : base(options)
        {
            _collection = GetCollection<User>("Users");
        }

        public Task<(int, string, UsageUserDTO?)> Update(UpdateDTO model)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, model.Id);
            var update = Builders<User>.Update.Set(user => user.ImageUrl, model.ImageUrl)
                                              .Set(user => user.Age, model.Age)
                                              .Set(user => user.City, model.City)
                                              .Set(user => user.Email, model.Email)
                                              .Set(user => user.Profession, model.Profession)
                                              .Set(user => user.Role, model.Role);
            _collection.UpdateOneAsync(filter, update);
        }
    }
}