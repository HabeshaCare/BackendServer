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
        private readonly IAdminService _adminService;
        private readonly IPatientService _patientService;
        private readonly IConfiguration _configuration;

        public AuthService(IMapper mapper, IDoctorService doctorService, IPatientService patientService, IAdminService adminService, IConfiguration configuration)
        {
            _configuration = configuration;

            _adminService = adminService;
            _doctorService = doctorService;
            _patientService = patientService;

            _mapper = mapper;
        }


        public async Task<(int, string, dynamic?)> Login(LoginDTO model)
        {

            // Checks if the user exists in the database from different collections.
            var adminTask = _adminService.GetAdminByEmail(model.Email);
            var doctorTask = _doctorService.GetDoctorByEmail(model.Email);
            var patientTask = _patientService.GetPatientByEmail(model.Email);

            // Used to call asynchronously and wait for all the tasks to complete which will be run in parallel.
            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Item3);

            //
            User user = admin as User ?? doctor as User ?? patient;

            bool ifUserNotFound = user == null;
            bool ifInvalidPassword = VerifyHashedPassword(model.Password, ifUserNotFound ? "" : user!.Password ?? "");

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
                    //TODO: This should be implemented for the Patients too.
                    Patient patient = _mapper.Map<Patient>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var (status, message, resultUser) = await _patientService.AddPatient(patient);
                        return CreateUser(resultUser!, model.Password, message, status);
                    }

                case UserRole.Doctor:
                    Doctor doctor = _mapper.Map<Doctor>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var (status, message, resultUser) = await _doctorService.AddDoctor(doctor);
                        return CreateUser(resultUser!, model.Password, message, status);
                    }
                case UserRole.SuperAdmin:
                    return (0, "This role is not allowed to be created", null);
                //TODO: This should be implemented for the different admins too.
                case UserRole.HealthCenterAdmin:
                case UserRole.LaboratoryAdmin:
                case UserRole.PharmacyAdmin:
                case UserRole.Reception:
                    Administrator admin = _mapper.Map<Administrator>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var (status, message, resultUser) = await _adminService.AddAdmin(admin);
                        return CreateUser(resultUser!, model.Password, message, status);
                    }
                default:
                    return (0, "Invalid Role", null);
            }
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

        private (int, string, UsageUserDTO?) CreateUser<T>(T resultUser, string password, string errorMessage, int status) where T : UserDTO
        {
            User user = _mapper.Map<User>(resultUser);

            if (status == 1 && resultUser != null)
            {
                // Maps DTO to User model and hashes the password.
                var hashedPassword = HashPassword(password);
                user.Password = hashedPassword;

                UsageUserDTO createdUser = _mapper.Map<UsageUserDTO>(user);

                return (1, "User created successfully", createdUser);
            }
            else
            {
                return (status, errorMessage, null);
            }
        }
    }
}