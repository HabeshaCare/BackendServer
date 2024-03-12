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
            var adminTask = _adminService.GetUserByEmail<Administrator>(model.Email);
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(model.Email);
            var patientTask = _patientService.GetUserByEmail<Patient>(model.Email);

            // Used to call asynchronously and wait for all the tasks to complete which will be run in parallel.
            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Item3);

            //
            User user = admin as User ?? doctor as User ?? patient;

            bool ifUserNotFound = user == null;
            bool ifValidPassword = VerifyHashedPassword(model.Password, ifUserNotFound ? "" : user!.Password ?? "");

            if (ifUserNotFound || !ifValidPassword)
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
            // Checks if the user exists in the database from different collections.
            var adminTask = _adminService.GetUserByEmail<Administrator>(model.Email);
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(model.Email);
            var patientTask = _patientService.GetUserByEmail<Patient>(model.Email);

            // Used to call asynchronously and wait for all the tasks to complete which will be run in parallel.
            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator _admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
            Doctor _doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
            Patient _patient = _mapper.Map<Patient>(patientTask.Result.Item3);

            //
            User _user = _admin as User ?? _doctor as User ?? _patient;

            bool ifUserFound = _user != null;

            if (ifUserFound)
                return (0, "User already exists", null);

            switch (model.Role)
            {
                case UserRole.Normal:
                    Patient patient = _mapper.Map<Patient>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(patient, model.Password);
                        if (resultUser != null)
                        {
                            return await _patientService.AddPatient(resultUser);
                        }
                        else
                        {
                            return (0, "Error while password hashing", null);
                        }
                    }

                case UserRole.Doctor:
                    Doctor doctor = _mapper.Map<Doctor>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(doctor, model.Password);
                        if (resultUser != null)
                        {
                            return await _doctorService.AddDoctor(resultUser);
                        }
                        else
                        {
                            return (0, "Error while password hashing", null);
                        }
                    }
                case UserRole.SuperAdmin:
                    return (0, "This role is not allowed to be created", null);

                case UserRole.HealthCenterAdmin:
                case UserRole.LaboratoryAdmin:
                case UserRole.PharmacyAdmin:
                case UserRole.Reception:
                    Administrator admin = _mapper.Map<Administrator>(model);
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(admin, model.Password);
                        if (resultUser != null)
                        {
                            return await _adminService.AddAdmin(resultUser);
                        }
                        else
                        {
                            return (0, "Error while password hashing", null);
                        }
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
            try
            {
                return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private T? AddPassword<T>(T user, string password) where T : User
        {
            if (user != null)
            {
                // Maps DTO to User model and hashes the password.
                var hashedPassword = HashPassword(password);
                user.Password = hashedPassword;

                return user;
            }
            else
            {
                return null;
            }
        }
    }
}