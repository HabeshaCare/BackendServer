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

        }

        public Task<(int, string, UsageUserDTO?)> Update(UpdateDTO model)
        {
            
        }
    }
}