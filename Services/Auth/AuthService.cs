using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Models.DTOs.UserDTOs;
using UserAuthentication.Utils;

namespace UserAuthentication.Services
{
    public class AuthService : MongoDBService, IAuthService
    {
        private readonly IMongoCollection<User> _collection;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public AuthService(IOptions<MongoDBSettings> options, IConfiguration configuration, IMapper mapper) : base(options)
        {
            _collection = GetCollection<User>("Users");
            _configuration = configuration;
            _mapper = mapper;
        }


        public async Task<(int, string, UsageUserDTO?)> Login(LoginDTO model)
        {
            var filterCondition = Builders<User>.Filter.Eq("Email", model.Email);
            var projection = Builders<User>.Projection
                .Include(u => u.Id)
                .Include(u => u.Age)
                .Include(u => u.Role)
                .Include(u => u.City)
                .Include(u => u.Email)
                .Include(u => u.Gender)
                .Include(u => u.Fullname)
                .Include(u => u.ImageUrl)
                .Include(u => u.Password)
                .Include(u => u.Profession)
                .Include(u => u.Phonenumber);
            User user = await _collection.Find(filterCondition).Project<User>(projection).FirstOrDefaultAsync();

            bool result = user != null && VerifyHashedPassword(model.Password, user.Password);

            bool ifUserNotFound = user == null;
            bool ifInvalidPassword = !result;

            if (ifUserNotFound || ifInvalidPassword)
                return (0, "Invalid Credentials", null);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, user!.Email),
                new(ClaimTypes.NameIdentifier, user!.Id!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            string token = GenerateToken(authClaims);
            UsageUserDTO foundUser = _mapper.Map<UsageUserDTO>(user);
            return (1, token, foundUser);
        }

        public async Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model)
        {
            var filterCondition = Builders<User>.Filter.Eq("Email", model.Email);
            User user = await _collection.Find(filterCondition).FirstOrDefaultAsync();

            if (user != null)
            {
                return (0, "User already exists", null);
            }

            user = _mapper.Map<User>(model);
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
            UsageUserDTO createdUser = _mapper.Map<UsageUserDTO>(user);

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
                Expires = DateTime.UtcNow.AddMinutes(30),
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