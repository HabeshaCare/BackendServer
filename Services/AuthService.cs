using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Utils;

namespace UserAuthentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoClient _client;
        private readonly IMongoCollection<User> _collection;
        private readonly IConfiguration _configuration;


        public AuthService(IOptions<MongoDBSettings> options, IConfiguration configuration)
        {
            _client = new MongoClient(options.Value.ConnectionUrl);
            var _database = _client.GetDatabase(options.Value.DBName);
            _collection = _database.GetCollection<User>("Users");
            _configuration = configuration;
        }


        public async Task<(int, string, UsageUserDTO?)> Login(LoginDTO model)
        {
            var filterCondition = Builders<User>.Filter.Eq("Email", model.Email);
            User user = await _collection.Find(filterCondition).FirstOrDefaultAsync();

            var result = VerifyHashedPassword(model.Password, user?.Password ?? "");

            bool ifUserNotFound = user == null;
            bool ifInvalidPassword = !result;

            if (ifUserNotFound || ifInvalidPassword)
                return (0, "Invalid Credentials", null);

            var userRoles = Enum.GetValues(typeof(UserRole));
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Email, user!.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };


            foreach (var userRole in userRoles)
                authClaims.Add(new(ClaimTypes.Role, userRole.ToString()!));

            string token = GenerateToken(authClaims);
            UsageUserDTO foundUser = new(user);
            return (1, token, foundUser);
        }

        public async Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model)
        {
            var filterCondition = Builders<User>.Filter.Eq("Email", model.Email);
            var userExists = await _collection.Find(filterCondition).FirstOrDefaultAsync();

            if (userExists != null)
            {
                return (0, "User already exists", null);
            }

            User user = new(model.Email, model.Phonenumber, model.Profession, model.Role);
            var hashedPassword = HashPassword(model.Password);

            user.Password = hashedPassword;

            try
            {
                await _collection.InsertOneAsync(user);
            }
            catch (Exception ex)
            {

                return (0, $"Database error creating the user: {ex.Message}", null);
            }
            UsageUserDTO createdUser = new(user);

            return (1, "User created successfully", createdUser);
        }

        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]!));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                // Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}