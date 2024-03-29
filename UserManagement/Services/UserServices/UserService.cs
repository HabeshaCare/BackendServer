using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.DTOs;
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
        public async Task<SResponseDTO<USD>> UploadProfilePic<USD>(string userId, IFormFile? image)
        {
            var filter = Builders<T>.Filter.Eq(user => user.Id, userId);
            try
            {
                string? imageUrl = null;
                if (image != null)
                {
                    var response = await _fileService.UploadFile(image, userId, "ProfilePics");

                    if (!response.Success)
                        return new() { StatusCode = response.StatusCode, Errors = response.Errors };

                    imageUrl = response.Data;
                }
                var userResponse = await GetUser(userId);

                if (!userResponse.Success)
                    return new() { StatusCode = userResponse.StatusCode, Errors = userResponse.Errors };

                var user = userResponse.Data;

                user!.ImageUrl = imageUrl;

                var options = new FindOneAndReplaceOptions<T>
                {
                    ReturnDocument = ReturnDocument.After
                };

                var rawUser = await _collection.FindOneAndReplaceAsync(filter, user, options);

                USD updatedUser = _mapper.Map<USD>(rawUser);
                return new() { StatusCode = 201, Message = "Profile Image Uploaded Successfully", Data = updatedUser, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }

        }


        protected async Task<SResponseDTO<T>> GetUser(string userId)
        {
            try
            {
                var result = await _collection.FindAsync(d => d.Id == userId);
                T? user = (await result.ToListAsync()).FirstOrDefault();
                return new() { StatusCode = 200, Data = user, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<SResponseDTO<USD>> GetUserById<USD>(string userId)
        {
            var response = await GetUser(userId);
            if (response.Success)
            {
                USD? foundUser = _mapper.Map<USD>(response.Data);
                return new() { StatusCode = 200, Message = response.Message, Data = foundUser, Success = true };
            }

            return new() { StatusCode = response.StatusCode, Errors = response.Errors };
        }

        // USD refers to the Usage DTO of a user
        public async Task<SResponseDTO<USD>> GetUserByEmail<USD>(string email)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("Email", email);
                T user = await _collection.Find(filterCondition).FirstOrDefaultAsync();

                if (user == null)
                    return new() { StatusCode = 404, Errors = new[] { "User not found" } };

                USD? foundUser = _mapper.Map<USD>(user);
                return new() { StatusCode = 200, Message = "User Found", Data = foundUser, Success = true };
            }
            catch (Exception)
            {
                return new() { StatusCode = 404, Errors = new[] { "User not found" } };
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<SResponseDTO<USD>> GetUserByVerificationToken<USD>(string token)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("VerificationToken", token);
                var foundUsers = await _collection.Find(filterCondition).ToListAsync();
                if (foundUsers.Count == 0)
                    return new() { StatusCode = 401, Errors = new[] { "Invalid Token" } };

                USD user = _mapper.Map<USD[]>(foundUsers)[0];
                return new() { StatusCode = 200, Message = "User Found", Data = user, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // USD refers to the Usage DTO of a user
        public async Task<SResponseDTO<USD>> GetUserByResetToken<USD>(string token)
        {
            try
            {
                var filterCondition = Builders<T>.Filter.Eq("PasswordResetToken", token);
                var foundUsers = await _collection.Find(filterCondition).ToListAsync();
                if (foundUsers.Count == 0)
                    return new() { StatusCode = 401, Errors = new[] { "Invalid Token" } };

                USD user = _mapper.Map<USD[]>(foundUsers)[0];
                return new() { StatusCode = 200, Message = "User Found", Data = user, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // USD refers to the Usage DTO of a user
        protected async Task<SResponseDTO<USD[]>> GetUsers<USD>(FilterDefinition<T> filterDefinition, int page, int size)
        {
            int skip = (page - 1) * size;
            try
            {
                var foundUsers = await _collection.Find(filterDefinition)
                    .Skip(skip)
                    .Limit(size)
                    .ToListAsync();

                if (foundUsers.Count == 0)
                    return new() { StatusCode = 404, Errors = new[] { "No matching users found" } };

                USD[] users = _mapper.Map<USD[]>(foundUsers);
                return new() { StatusCode = 200, Message = $"Found {foundUsers.Count} matching users", Data = users, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // UD refers to the Update DTO of a user
        // USD refers to the Usage DTO of a user
        protected async Task<SResponseDTO<USD>> UpdateUser<UD, USD>(UD userDTO, string userId)
        {
            try
            {
                var response = await GetUser(userId);
                if (response.Success)
                {
                    var user = response.Data;
                    _mapper.Map(userDTO, response.Data);

                    var filter = Builders<T>.Filter.And(
                        Builders<T>.Filter.Eq(u => u.Id, userId));

                    var options = new FindOneAndReplaceOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After // or ReturnDocument.Before
                    };

                    var result = await _collection.FindOneAndReplaceAsync(filter, user!, options);


                    USD updatedUserDTO = _mapper.Map<USD>(result);

                    if (result == null)
                    {
                        return new() { StatusCode = 500, Errors = new[] { "Error updating user" } };
                    }
                    return new() { StatusCode = 200, Message = "User profile updated successfully", Data = updatedUserDTO, Success = true };
                }
                return new() { StatusCode = response.StatusCode, Errors = response.Errors };

            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // USD refers to the Usage DTO of a user
        protected async Task<SResponseDTO<USD>> AddUser<USD>(T user)
        {
            try
            {
                var response = await GetUserByEmail<T>(user.Email ?? "");

                if (response.Success)
                    return new() { StatusCode = 409, Errors = new[] { "User already exists" } };

                await _collection.InsertOneAsync(user);
                USD createdUser = _mapper.Map<USD>(user);

                return new() { StatusCode = 201, Message = "User created successfully", Data = createdUser, Success = true };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        // AD refers to the Registration DTO of a user
        // USD refers to the Usage DTO of a user
        public async Task<SResponseDTO<string>> UpdatePassword<USD>(string id, string newHashedPassword)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
                var update = Builders<T>.Update.Set("Password", newHashedPassword);

                var result = await _collection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount > 0)
                    return new() { StatusCode = 200, Message = "Password Reset successfully", Success = true };
                else
                    return new() { StatusCode = 500, Errors = new[] { "User doesn't exist" } };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }
    }
}

