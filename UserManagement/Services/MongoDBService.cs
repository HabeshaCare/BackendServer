using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class MongoDBService<T> where T : User
    {
        private readonly IMongoClient _client;
        protected readonly IMongoDatabase database;
        private readonly IMongoCollection<T> _collection;
        private readonly IMapper _mapper;
        public MongoDBService(IOptions<MongoDBSettings> options, IMapper mapper)
        {
            _client = new MongoClient(options.Value.ConnectionUrl);
            database = _client.GetDatabase(options.Value.DBName);
            _collection = database.GetCollection<T>($"{typeof(T).Name}s");
            _mapper = mapper;
        }

        protected IMongoCollection<T> GetCollection(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        // USD refers to the Usage DTO of a user
        private async Task<(int, string?, T?)> GetUser(string userId)
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
        public async Task<(int, string?, USD[])> GetUsers<USD>(int page, int size, UserRole role = UserRole.Normal)
        {
            var filterBuilder = Builders<T>.Filter;
            var filterDefinition = filterBuilder.Empty;

            filterDefinition &= filterBuilder.Eq("Role", role);

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
        public async Task<(int, string, USD?)> UpdateUser<UD, USD>(UD userDTO, string userId)
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

                    return (1, "user profile updated successfully. Status set to unverified until approved by Admin", updatedUserDTO);
                }

                return (0, "user not found", default(USD));

            }
            catch (Exception ex)
            {
                return (0, ex.Message, default(USD));
            }
        }
    }
}