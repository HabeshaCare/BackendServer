using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class MongoDBService
    {
        private readonly IMongoClient _client;
        protected readonly IMongoDatabase database;
        public MongoDBService(IOptions<MongoDBSettings> options)
        {
            _client = new MongoClient(options.Value.ConnectionUrl);
            database = _client.GetDatabase(options.Value.DBName);
        }

        protected IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName);
        }

        protected IMongoCollection<User> GetCollection(UserRole role)
        {
            switch (role)
            {
                case UserRole.Normal:
                    return (IMongoCollection<User>)GetCollection<Patient>("Patients");
                case UserRole.Doctor:
                    return (IMongoCollection<User>)GetCollection<Doctor>("Doctors");
                case UserRole.Admin:
                    return (IMongoCollection<User>)GetCollection<Administrator>("Administrators");
                default:
                    return GetCollection<User>("Users");
            }
        }
    }
}