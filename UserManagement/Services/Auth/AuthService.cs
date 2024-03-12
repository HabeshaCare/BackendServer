using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.DTOs.UserDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.EmailService;
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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(IMapper mapper, IDoctorService doctorService, IPatientService patientService, IAdminService adminService, IEmailService emailService, IConfiguration configuration)
        {
            _configuration = configuration;

            _adminService = adminService;
            _doctorService = doctorService;
            _patientService = patientService;
            _emailService = emailService;

            _mapper = mapper;
        }


        public async Task<(int, string, UsageUserDTO?)> Login(LoginDTO model)
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

            bool userNotFound = user == null;
            bool validPassword = VerifyHashedPassword(model.Password, userNotFound ? "" : user!.Password ?? "");
            bool userNotVerified = user?.VerifiedAt == null;

            if (userNotFound || !validPassword)
                return (0, "Invalid Credentials", null);

            if (userNotVerified)
                return (0, "Account not verified", null);

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

        public async Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model)
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
            (int, string?, UsageUserDTO?) result;

            if (ifUserFound)
                return (0, "User already exists", null);

            switch (model.Role)
            {
                case UserRole.Normal:
                    Patient patient = _mapper.Map<Patient>(model);
                    patient.VerificationToken = CreateRandomToken();
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(patient, model.Password);
                        if (resultUser != null)
                        {
                            result = await _patientService.AddPatient(resultUser);
                            break;
                        }
                        else
                        {
                            return (0, "Error while password hashing", null);
                        }
                    }

                case UserRole.Doctor:
                    Doctor doctor = _mapper.Map<Doctor>(model);
                    doctor.VerificationToken = CreateRandomToken();
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(doctor, model.Password);
                        if (resultUser != null)
                        {
                            result = await _doctorService.AddDoctor(resultUser);
                            break;
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
                    admin.VerificationToken = CreateRandomToken();
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(admin, model.Password);
                        if (resultUser != null)
                        {
                            result = await _adminService.AddAdmin(resultUser);
                            break;
                        }
                        else
                        {
                            return (0, "Error while password hashing", null);
                        }
                    }
                default:
                    return (0, "Invalid Role", null);
            }

            var user = result.Item3;
            bool registrationSuccessful = user?.VerificationToken != string.Empty;

            if (registrationSuccessful)
            {
                string verificationUrl = $"https://localhost:5072/verifyEmail/?token={user?.VerificationToken}";

                EmailDTO email = new()
                {
                    To = model.Email,
                    Subject = "Email Verification",
                    Body = $"Please visit the following url to verify your email.{verificationUrl}"
                };
                bool emailSent = _emailService.SendEmail(email);

                if (emailSent)
                {
                    return (1, "Registration Complete. Check email for verification link to verify your account.", user);
                }
                else
                {
                    return (1, "Registration Complete but couldn't sent verification email ", user);
                }
            }
            else
            {
                return (0, "Something went wrong on registration", null);
            }
        }

        public async Task<(int, string, UsageUserDTO?)> VerifyEmail(string token)
        {
            try
            {
                var adminTask = _adminService.GetUserByVerificationToken<Administrator>(token);
                var doctorTask = _doctorService.GetUserByVerificationToken<Doctor>(token);
                var patientTask = _patientService.GetUserByVerificationToken<Patient>(token);

                await Task.WhenAll(adminTask, doctorTask, patientTask);

                Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
                Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
                Patient patient = _mapper.Map<Patient>(patientTask.Result.Item3);

                User user = admin as User ?? doctor as User ?? patient;

                bool userNotFound = user == null;

                if (userNotFound)
                    return (0, "Invalid Token", null);

                user!.VerifiedAt = DateTime.Now;
                user.VerificationToken = string.Empty;

                string successMessage = "User Verified";

                return await UpdateUser(user, successMessage);

            }
            catch (Exception ex)
            {
                return (0, ex.Message, null);
            }
        }

        public async Task<(int, string, UsageUserDTO?)> ForgotPassword(string email)
        {
            var adminTask = _adminService.GetUserByEmail<Administrator>(email);
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(email);
            var patientTask = _patientService.GetUserByEmail<Patient>(email);

            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Item3);

            User user = admin as User ?? doctor as User ?? patient;

            if (user == null)
            {
                return (0, "User not found.", null);
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            string successMessage = "Password reset link sent";
            return await UpdateUser(user, successMessage);

        }

        public async Task<(int, string, UsageUserDTO?)> ResetPassword(UserResetPasswordDTO request)
        {
            var adminTask = _adminService.GetUserByResetToken<Administrator>(request.Token);
            var doctorTask = _doctorService.GetUserByResetToken<Doctor>(request.Token);
            var patientTask = _patientService.GetUserByResetToken<Patient>(request.Token);

            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Item3);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Item3);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Item3);

            User user = admin as User ?? doctor as User ?? patient;

            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return (0, "Invalid token.", null);
            }

            user.Password = HashPassword(request.Password);

            user.PasswordResetToken = string.Empty;
            user.ResetTokenExpires = null;

            string successMessage = "Password reset link sent";
            return await UpdateUser(user, successMessage);

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

        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
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

        private async Task<(int, string, UsageUserDTO?)> UpdateUser(User user, string successMessage = "User updated successfully")
        {

            switch (user.Role)
            {
                case UserRole.Normal:
                    {
                        UpdatePatientDTO updatedUser = _mapper.Map<UpdatePatientDTO>(user);
                        var (status, message, verifiedUser) = await _patientService.UpdatePatient(updatedUser, user.Id!);
                        if (status == 1 && verifiedUser != null)
                            return (1, successMessage, verifiedUser);
                        return (0, message, null);
                    }
                case UserRole.Doctor:
                    {
                        UpdateDoctorDTO updatedUser = _mapper.Map<UpdateDoctorDTO>(user);
                        var (status, message, verifiedUser) = await _doctorService.UpdateDoctor(updatedUser, user.Id!);
                        if (status == 1 && verifiedUser != null)
                            return (1, successMessage, verifiedUser);
                        return (0, message, null);
                    }
                case UserRole.SuperAdmin:
                case UserRole.HealthCenterAdmin:
                case UserRole.LaboratoryAdmin:
                case UserRole.PharmacyAdmin:
                case UserRole.Reception:
                    {
                        UpdateAdminDTO updatedUser = _mapper.Map<UpdateAdminDTO>(user);
                        var (status, message, verifiedUser) = await _adminService.UpdateAdmin((updatedUser as UpdateAdminDTO)!, user.Id!);
                        if (status == 1 && verifiedUser != null)
                            return (1, successMessage, verifiedUser);
                        return (0, message, null);
                    }
                default:
                    return (0, "Invalid Role", null);
            }
        }
    }
}