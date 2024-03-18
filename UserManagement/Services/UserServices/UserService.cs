using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class UserService<T> : MongoDBService, IUserService where T : User
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly IFileService _fileService;
        protected readonly IMapper _mapper;
        public UserService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options)
        {
            _collection = GetCollection<T>($"{typeof(T).Name}s");
            _fileService = fileService;
            _mapper = mapper;
        }

        //USD refers to the Usage DTO of a user
        public async Task<(int, string, USD?)> UploadProfilePic<USD>(string userId, IFormFile? image)
        {
            var filter = Builders<T>.Filter.Eq(user => user.Id, userId);
            try
            {
                string? imageUrl = null;
                if (image != null)
                {
                    var (fileStatus, fileMessage, filePath) = await _fileService.UploadFile(image, userId, "ProfilePics");
                    if (fileStatus == 1 || filePath == null)
                        return (fileStatus, fileMessage, default(USD));

                    imageUrl = filePath;
                }
                var (userStatus, userMessage, user) = await GetUser(userId);

                if (userStatus == 0 || user == null)
                    return (userStatus, userMessage ?? "User doesn't Exist", default(USD));

                user.ImageUrl = imageUrl;
                var options = new FindOneAndReplaceOptions<T>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var rawUser = await _collection.FindOneAndReplaceAsync(filter, user, options);

                USD updatedUser = _mapper.Map<USD>(rawUser);
                return (1, "Profile Image Uploaded Successfully", updatedUser);
            }
            catch (Exception ex)
            {
                return (1, ex.Message, default(USD));
            }

        }


        protected async Task<(int, string?, T?)> GetUser(string userId)
        {
            try
            {
                var result = await _collection.FindAsync(d => d.Id == userId);
                T? user = (await result.ToListAsync()).FirstOrDefault();
                return (1, null, user);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<(int, string?, USD?)> GetUserById<USD>(string userId)
        {
            var (status, message, user) = await GetUser(userId);
            if (status == 1 && user != null)
            {
                USD? foundUser = _mapper.Map<USD>(user);
                return (status, message, foundUser);
            }

            return (status, message, default(USD));
        }

        // USD refers to the Usage DTO of a user
        public async Task<(int, string?, USD?)> GetUserByEmail<USD>(string email)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("Email", email);
                T user = await _collection.Find(filterCondition).FirstOrDefaultAsync();
                USD? foundUser = _mapper.Map<USD>(user);
                return (1, "Found User", foundUser);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<(int, string?, USD?)> GetUserByVerificationToken<USD>(string token)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("VerificationToken", token);
                var foundUsers = await _collection.Find(filterCondition).ToListAsync();
                if (foundUsers.Count == 0)
                    return (0, "Invalid Token", default(USD));

                USD user = _mapper.Map<USD[]>(foundUsers)[0];
                return (1, "Found User", user);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<(int, string?, USD?)> GetUserByResetToken<USD>(string token)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("PasswordResetToken", token);
                var foundUsers = await _collection.Find(filterCondition).ToListAsync();
                if (foundUsers.Count == 0)
                    return (0, "Invalid Token", default(USD));

                USD user = _mapper.Map<USD[]>(foundUsers)[0];
                return (1, "Found User", user);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // USD refers to the Usage DTO of a user
        protected async Task<(int, string?, USD[])> GetUsers<USD>(FilterDefinition<T> filterDefinition, int page, int size)
        {
            int skip = (page - 1) * size;
            try
            {
                var foundUsers = await _collection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundUsers.Count == 0)
                    return (0, "No matching users found", Array.Empty<USD>());

                USD[] users = _mapper.Map<USD[]>(foundUsers);
                return (1, $"Found {foundUsers.Count} matching users", users);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, Array.Empty<USD>())!;
            }
        }



        // UD refers to the Update DTO of a user
        // USD refers to the Usage DTO of a user
        protected async Task<(int, string, USD?)> UpdateUser<UD, USD>(UD userDTO, string userId)
        {
            try
            {
                var (status, message, user) = await GetUser(userId);
                if (status == 1 && user != null)
                {
                    _mapper.Map(userDTO, user);

                    var filter = Builders<T>.Filter.And(
                        Builders<T>.Filter.Eq(u => u.Id, userId));

                    var options = new FindOneAndReplaceOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After // or ReturnDocument.Before
                    };

                    var result = await _collection.FindOneAndReplaceAsync(filter, user, options);


                    USD updatedUserDTO = _mapper.Map<USD>(result);

                    if (result == null)
                    {
                        return (0, "Error updating user", default(USD));
                    }

                    return (1, "User profile updated successfully.", updatedUserDTO);
                }

                return (0, "User not found", default(USD));

            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // AD refers to the Registration DTO of a user
        // USD refers to the Usage DTO of a user
        protected async Task<(int, string, USD?)> AddUser<USD>(T user)
        {
            try
            {
                var (status, message, foundUser) = await GetUserByEmail<T>(user.Email ?? "");

                if (status == 1 && foundUser != null)
                    return (0, "User already exists", default(USD));

                await _collection.InsertOneAsync(user);
                USD createdUser = _mapper.Map<USD>(user);

                return (1, "User created successfully", createdUser);
            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }

        // AD refers to the Registration DTO of a user
        // USD refers to the Usage DTO of a user
        public async Task<(int, string)> UpdatePassword<USD>(string id, string newHashedPassword)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<T>.Update.Set("Password", newHashedPassword);

                var result = await _collection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount > 0)
                    return (1, "Password Reset successfully");
                else
                    return (0, "User doesn't exist");
            }
            catch (Exception ex)
            {
                return (0, ex.Message);
            }
        }

        // private async Task<(int, string?, User)> GetUser(string id)
        // {
        //     try
        //     {
        //         var rawUser = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        //         return (1, null, rawUser);
        //     }
        //     catch (FormatException)
        //     {
        //         var rawUser = await _doctorCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
        //         return (1, null, rawUser);
        //     }
        //     catch (Exception ex)
        //     {
        //         return (0, ex.Message, null);

        //     }
        // }

        // public async Task<(int, string?, UsageUserDTO?)> GetUserById(string id)
        // {
        //     var (status, message, rawUser) = await GetUser(id);
        //     if (status == 0 || rawUser == null)
        //         return (0, message, null);

        //     var foundUser = _mapper.Map<UsageUserDTO>(rawUser);
        //     return (1, "User Found", foundUser);
        // }

        //        public async Task<(int, string, UsageUserDTO?)> UpdateUser(UpdateUserDTO model, string userId)
        // {
        //     var filter = Builders<User>.Filter.Eq(user => user.Id, userId);
        //     try
        //     {
        //         if (model != null)
        //         {

        //             var (userStatus, userMessage, user) = await GetUser(userId);

        //             if (userStatus == 0 || user == null)
        //                 return (userStatus, userMessage ?? "User doesn't Exist", null);

        //             _mapper.Map(model, user);

        //             var options = new FindOneAndReplaceOptions<User>
        //             {
        //                 ReturnDocument = ReturnDocument.After
        //             };

        //             var rawUser = await _collection.FindOneAndReplaceAsync(filter, user, options);

        //             UsageUserDTO updatedUser = _mapper.Map<UsageUserDTO>(rawUser);
        //             return (1, "User updated Successfully", updatedUser);
        //         }
        //         return (0, "Invalid Input", null);
        //     }
        //     catch (Exception ex)
        //     {
        //         return (1, ex.Message, null);
        //     }

        // }
    }
}

