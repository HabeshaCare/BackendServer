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
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IDoctorService _doctorService;

        public AuthService(IMapper mapper, IDoctorService doctorService)
        {
            _doctorService = doctorService;
            _mapper = mapper;
        }


        public async Task<(int, string, dynamic?)> Login(LoginDTO model)
        {

            // Extracts user information and verifies the password.
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(model.Email);
            //Todo: Add the user service to get the user by email.

            await Task.WhenAll(doctorTask);

            User user = _mapper.Map<User>(doctorTask.Result.Item3);

            bool ifUserNotFound = user == null;
            bool ifInvalidPassword = VerifyHashedPassword(model.Password, user.Password ?? "");

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

            switch (model.Role)
            {
                case UserRole.Normal:
                    // user = _mapper.Map<Patient>(model);
                    break;
                case UserRole.Doctor:
                    var (status, message, doctor) = await _doctorService.GetUserByEmail<Doctor>(model.Email!);
                    user = _mapper.Map<Doctor>(model);
                    //Register the user profile
                    var (status, message, resultUser) = await _doctorService.AddUser<UsageUserDTO, Doctor>(user);
                    break;
                case UserRole.Admin:
                    user = _mapper.Map<Administrator>(model);
                    break;
                default:
                    return (0, "Invalid Role", null);
            }
            if (user != null)
            {
                return (0, "User already exists", null);
            }

            //Update the role specific information in a different collection

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