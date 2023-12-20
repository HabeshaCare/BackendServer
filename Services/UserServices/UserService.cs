using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Services.FileServices;
using UserAuthentication.Utils;

namespace UserAuthentication.Services.UserServices
{
    public class UserService : MongoDBService, IUserService
    {
        private readonly IMongoCollection<User>_collection;
        private readonly IFileService _fileService;
        public UserService(IOptions<MongoDBSettings> options, IFileService fileService) : base(options)
        {
            _collection = GetCollection<User>("Users");
            _fileService = fileService;
        }

        public async Task<(int, string, UsageUserDTO?)> Update(UpdateDTO model)
        {
            string? imageUrl = null;
            var filter = Builders<User>.Filter.Eq(user => user.Id, model.Id);
            try
            {
                if(model.Image != null)
                {
                    var (status, message, filePath) = await _fileService.UploadFile(model.Image, model.Id!, "ProfilePics");
                    if(status == 1 || filePath == null)
                        return (status, message, null);
                    imageUrl = filePath;
                }
            }
            catch(Exception ex)
            {
                return (1, ex.Message, null);
            }

            var update = GetUpdateDefinition(model, imageUrl);

            var options = new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After
            };
            try
            {
                var rawUser = await _collection.FindOneAndUpdateAsync(filter, update, options);                
                UsageUserDTO updatedUser = new();
                updatedUser.MapFromUser(rawUser);
                return (1, "User updated Successfully", updatedUser);
            }
            catch (Exception ex)
            { 
                return (0, ex.Message, null) ;
            }
            
        }

        public UpdateDefinition<User> GetUpdateDefinition(UpdateDTO model, string? imageUrl)
        {
            var update = Builders<User>.Update.Set(user => user.Id, model.Id);
            
            if(model.Age != null)
                update.Set(user => user.Age, model.Age);
            
            if(model.Email != null)
                update.Set(user => user.Email, model.Email);

            if(model.City != null)
                update.Set(user => user.City, model.City);

            if(model.Profession != null)
                update.Set(user => user.Profession, model.Profession);
            
            if(model.Role != null)
                update.Set(user => user.Role, model.Role);

            if(imageUrl != null)
                update.Set(user => user.ImageUrl, imageUrl);

            return update;
        }
    }
}