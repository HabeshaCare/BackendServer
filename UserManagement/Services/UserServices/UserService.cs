using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.FileServices;
using UserManagement.Utils;
using ZstdSharp.Unsafe;

namespace UserManagement.Services.UserServices
{
    public class UserService : MongoDBService, IUserService
    {
        private readonly IMongoCollection<User> _collection;
        private readonly IMongoCollection<Doctor> _doctorCollection;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public UserService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options)
        {
            _collection = GetCollection<User>("Users");
            _doctorCollection = GetCollection<Doctor>("Users");
            _fileService = fileService;
            _mapper = mapper;
        }

        private async Task<(int, string?, User)> GetUser(string id)
        {
            try
            {
                var rawUser = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
                return (1, null, rawUser);
            }
            catch (FormatException)
            {
                var rawUser = await _doctorCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                return (1, null, rawUser);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);

            }
        }

        public async Task<(int, string?, UsageUserDTO?)> GetUserById(string id)
        {
            var (status, message, rawUser) = await GetUser(id);
            if (status == 0 || rawUser == null)
                return (0, message, null);

            var foundUser = _mapper.Map<UsageUserDTO>(rawUser);
            return (1, "User Found", foundUser);
        }

        public async Task<(int, string, UsageUserDTO?)> UploadProfile(string userId, IFormFile? image)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            try
            {
                string? imageUrl = null;
                if (image != null)
                {
                    var (fileStatus, fileMessage, filePath) = await _fileService.UploadFile(image, userId, "ProfilePics");
                    if (fileStatus == 1 || filePath == null)
                        return (fileStatus, fileMessage, null);

                    imageUrl = filePath;
                }
                var (userStatus, userMessage, user) = await GetUser(userId);

                if (userStatus == 0 || user == null)
                    return (userStatus, userMessage ?? "User doesn't Exist", null);

                user.ImageUrl = imageUrl;
                var options = new FindOneAndReplaceOptions<User>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var rawUser = await _collection.FindOneAndReplaceAsync(filter, user, options);

                UsageUserDTO updatedUser = _mapper.Map<UsageUserDTO>(rawUser);
                return (1, "Profile Image Uploaded Successfully", updatedUser);
            }
            catch (Exception ex)
            {
                return (1, ex.Message, null);
            }

        }

        public async Task<(int, string, UsageUserDTO?)> UpdateUser(UpdateUserDTO model, string userId)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            try
            {
                if (model != null)
                {

                    var (userStatus, userMessage, user) = await GetUser(userId);

                    if (userStatus == 0 || user == null)
                        return (userStatus, userMessage ?? "User doesn't Exist", null);

                    _mapper.Map(model, user);

                    var options = new FindOneAndReplaceOptions<User>
                    {
                        ReturnDocument = ReturnDocument.After
                    };

                    var rawUser = await _collection.FindOneAndReplaceAsync(filter, user, options);

                    UsageUserDTO updatedUser = _mapper.Map<UsageUserDTO>(rawUser);
                    return (1, "User updated Successfully", updatedUser);
                }
                return (0, "Invalid Input", null);
            }
            catch (Exception ex)
            {
                return (1, ex.Message, null);
            }

        }
    }
}