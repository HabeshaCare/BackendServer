using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.UserServices;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class AuthService : MongoDBService, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMongoCollection<User> _collection;
        private readonly IDoctorService _doctorService;


        public AuthService(IOptions<MongoDBSettings> options, IConfiguration configuration, IMapper mapper, IDoctorService doctorService) : base(options)
        {
            _collection = GetCollection<User>("Users");
            _configuration = configuration;
            _doctorService = doctorService;
            _mapper = mapper;
        }


        public async Task<(int, string, dynamic?)> Login(LoginDTO model)
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
                .Include(u => u.Phonenumber);
            User user = await _collection.Find(filterCondition).Project<User>(projection).FirstOrDefaultAsync();

            // Extracts user information and verifies the password.
            bool result = user != null && VerifyHashedPassword(model.Password, user.Password);

            bool ifUserNotFound = user == null;
            bool ifInvalidPassword = !result;

            if (ifUserNotFound || ifInvalidPassword)
                return (0, "Invalid Credentials", null);

            // Generates authentication claims and token.
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, user!.Email!),
                new(ClaimTypes.NameIdentifier, user!.Id!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            string token = GenerateToken(authClaims);
            UsageUserDTO foundUser = _mapper.Map<UsageUserDTO>(user);
            return (1, token, foundUser);
        }

        public async Task<(int, string, dynamic?)> Registration(RegistrationDTO model)
        {

            var filterCondition = Builders<User>.Filter.Eq("Email", model.Email);
            dynamic user = await _collection.Find(filterCondition).FirstOrDefaultAsync();

            if (user != null)
            {
                return (0, "User already exists", null);
            }

            user = _mapper.Map<User>(model);

            //Register the user profile
            try
            {
                await _collection.InsertOneAsync(user);
            }
            catch (Exception ex)
            {

                return (0, $"Database error creating the user: {ex.Message}", null);
            }

            //Update the role specific information in a different collection
            switch (model.Role)
            {
                case UserRole.Normal:
                    user = _mapper.Map<Patient>(model);
                    break;
                case UserRole.Doctor:
                    user = _mapper.Map<Doctor>(model);
                    UpdateDoctorDTO doctorDTO = _mapper.Map<UpdateDoctorDTO>(user);
                    var response = await _doctorService.UpdateDoctor(doctorDTO, user.Id);
                    if (response.Item1 == 0)
                    {
                        return (0, response.Item2, null);
                    }
                    break;
                case UserRole.Admin:
                    user = _mapper.Map<Administrator>(model);
                    break;
                default:
                    return (0, "Invalid Role", null);
            }

            // Maps DTO to User model and hashes the password.
            var hashedPassword = HashPassword(model.Password);
            user.Password = hashedPassword;

            UsageUserDTO createdUser = _mapper.Map<UsageUserDTO>(user);

            return (1, "User created successfully", createdUser);
        }

        //Generates a JWT authentication token based on provided claims.
        public string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]!));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                // Expires = DateTime.UtcNow.AddMinutes(30),
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