using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.DTOs.PatientDTOs;
using UserManagement.DTOs.UserDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;
using UserManagement.Services.EmailService;
using UserManagement.Services.UserServices;

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


        public async Task<SResponseDTO<UsageUserDTO>> Login(LoginDTO model)
        {

            // Checks if the user exists in the database from different collections.
            var adminTask = _adminService.GetUserByEmail<Administrator>(model.Email);
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(model.Email);
            var patientTask = _patientService.GetUserByEmail<Patient>(model.Email);

            // Used to call asynchronously and wait for all the tasks to complete which will be run in parallel.
            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Data);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Data);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Data);

            //
            User user = admin as User ?? doctor as User ?? patient;

            bool userNotFound = user == null;
            bool validPassword = VerifyHashedPassword(model.Password, userNotFound ? "" : user!.Password ?? "");
            bool userNotVerified = user?.VerifiedAt == null;

            if (userNotFound || !validPassword)
                return new() { StatusCode = 401, Errors = new[] { "Invalid Credentials" } };

            if (userNotVerified)
                return new() { StatusCode = 401, Errors = new[] { "Account not verified" } };

            // Generates authentication claims and token.
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Email, user!.Email!),
                new(ClaimTypes.NameIdentifier, user!.Id!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new Claim("InstitutionId", admin?.InstitutionId.ToString() ?? string.Empty),
            };

            string token = GenerateToken(authClaims);
            UsageUserDTO foundUser = _mapper.Map<UsageUserDTO>(user);
            return new() { StatusCode = 200, Message = "Login successful", Data = foundUser, Token = token };
        }

        public async Task<SResponseDTO<UsageUserDTO>> Registration(RegistrationDTO model)
        {
            SResponseDTO<User> result;

            switch (model.Role)
            {
                case UserRole.Patient:
                    Patient patient = _mapper.Map<Patient>(model);
                    patient.VerificationToken = CreateRandomToken();
                    {
                        //The curly braces are used to limit the scope of the variable declaration
                        var resultUser = AddPassword(patient, model.Password);
                        if (resultUser != null)
                        {
                            var response = await _patientService.AddPatient(resultUser);
                            result = new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Errors = response.Errors, Success = response.Success };
                            break;
                        }
                        else
                        {
                            return new() { StatusCode = 500, Errors = new[] { "Error while password hashing" } };
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
                            var response = await _doctorService.AddDoctor(resultUser);
                            result = new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Errors = response.Errors, Success = response.Success };
                            break;
                        }
                        else
                        {
                            return new() { StatusCode = 500, Errors = new[] { "Error while hashing password" } };
                        }
                    }
                case UserRole.SuperAdmin:
                    return new() { StatusCode = 403, Errors = new[] { "This role is not allowed to be created" } };


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
                            var response = await _adminService.AddAdmin(resultUser);

                            result = new() { StatusCode = response.StatusCode, Message = response.Message, Data = response.Data, Errors = response.Errors, Success = response.Success };

                            break;
                        }
                        else
                        {
                            return new() { StatusCode = 500, Errors = new[] { "Error while password hashing" } };

                        }
                    }
                default:
                    return new() { StatusCode = 400, Errors = new[] { "Invalid Role" } };

            }

            var user = result.Data;
            bool registrationSuccessful = result.Success;
            var updatedUser = _mapper.Map<UsageUserDTO>(user);

            if (registrationSuccessful)
            {
                string verificationUrl = $"{_configuration["VerificationUrl"]}/?token={user?.VerificationToken}";

                EmailDTO email = new()
                {
                    To = model.Email!,
                    Subject = "Email Verification",
                    Body = $"Please visit the following url to verify your email.{verificationUrl}"
                };
                bool emailSent = _emailService.SendEmail(email);

                if (emailSent)
                {
                    return new() { StatusCode = 201, Message = "Registration Complete. Check email for verification link to verify your account.", Data = updatedUser };
                }
                else
                {
                    return new() { StatusCode = 201, Message = "Registration Complete but couldn't sent verification email.", Data = updatedUser };
                }
            }
            else
            {
                return new() { StatusCode = result.StatusCode, Errors = result.Errors };
            }
        }

        public async Task<SResponseDTO<UsageUserDTO>> VerifyEmail(string token)
        {
            try
            {
                var adminTask = _adminService.GetUserByVerificationToken<UsageAdminDTO>(token);
                var doctorTask = _doctorService.GetUserByVerificationToken<UsageDoctorDTO>(token);
                var patientTask = _patientService.GetUserByVerificationToken<UsagePatientDTO>(token);

                await Task.WhenAll(adminTask, doctorTask, patientTask);

                Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Data);
                Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Data);
                Patient patient = _mapper.Map<Patient>(patientTask.Result.Data);

                User user = admin as User ?? doctor as User ?? patient;

                bool userNotFound = user == null;

                if (userNotFound)
                    return new() { StatusCode = 401, Errors = new[] { "Invalid Token" } };

                user!.VerifiedAt = DateTime.Now;
                user.VerificationToken = string.Empty;

                var response = await UpdateUser(user);
                if (response.Success)
                {
                    return new() { StatusCode = response.StatusCode, Message = "Email verified", Data = response.Data, Success = true };
                }
                return new() { StatusCode = 400, Errors = response.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<UsageUserDTO>> ForgotPassword(string email)
        {
            var adminTask = _adminService.GetUserByEmail<Administrator>(email);
            var doctorTask = _doctorService.GetUserByEmail<Doctor>(email);
            var patientTask = _patientService.GetUserByEmail<Patient>(email);

            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Data);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Data);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Data);

            User user = admin as User ?? doctor as User ?? patient;

            if (user == null)
            {
                return new() { StatusCode = 404, Errors = new[] { "User not found" } };
            }

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);

            string resetUrl = $"https://localhost:5072/resetPassword/?token={user.PasswordResetToken}";

            EmailDTO emailRequest = new()
            {
                To = user.Email ?? "",
                Subject = "Password Reset",
                Body = $"Please visit the following url to reset your password.{resetUrl}"
            };

            var response = await UpdateUser(user);
            bool emailSent = _emailService.SendEmail(emailRequest);
            if (emailSent && response.Success)
            {
                return new() { StatusCode = 201, Message = "Check email for password reset link.", Data = response.Data };
            }
            else
            {
                return new() { StatusCode = 500, Errors = response.Errors };
            }


        }

        public async Task<SResponseDTO<string>> ResetPassword(UserResetPasswordDTO request)
        {
            var adminTask = _adminService.GetUserByResetToken<Administrator>(request.Token);
            var doctorTask = _doctorService.GetUserByResetToken<Doctor>(request.Token);
            var patientTask = _patientService.GetUserByResetToken<Patient>(request.Token);

            await Task.WhenAll(adminTask, doctorTask, patientTask);

            Administrator admin = _mapper.Map<Administrator>(adminTask.Result.Data);
            Doctor doctor = _mapper.Map<Doctor>(doctorTask.Result.Data);
            Patient patient = _mapper.Map<Patient>(patientTask.Result.Data);

            User user = admin as User ?? doctor as User ?? patient;

            if (user == null || user.ResetTokenExpires < DateTime.Now)
            {
                return new() { StatusCode = 401, Message = "Invalid token." };
            }

            user.Password = HashPassword(request.Password);

            user.PasswordResetToken = string.Empty;
            user.ResetTokenExpires = null;

            await UpdateUser(user);
            var response = await UpdatePassword(user);

            return new() { StatusCode = response.StatusCode, Message = response.Message, Errors = response.Errors, Success = response.Success };
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

        private async Task<SResponseDTO<UsageUserDTO>> UpdateUser(User user)
        {

            switch (user.Role)
            {
                case UserRole.Patient:
                    {
                        UpdatePatientDTO updatedUser = _mapper.Map<UpdatePatientDTO>(user);
                        var response = await _patientService.UpdatePatient(updatedUser, user.Id!);
                        var verifiedUser = _mapper.Map<UsageUserDTO>(response.Data);

                        return new() { StatusCode = response.StatusCode, Message = response.Message, Data = verifiedUser, Errors = response.Errors, Success = response.Success };
                    }
                case UserRole.Doctor:
                    {
                        UpdateDoctorDTO updatedUser = _mapper.Map<UpdateDoctorDTO>(user);
                        var response = await _doctorService.UpdateDoctor(updatedUser, user.Id!);
                        var verifiedUser = _mapper.Map<UsageUserDTO>(response.Data);

                        return new() { StatusCode = response.StatusCode, Message = response.Message, Data = verifiedUser, Errors = response.Errors, Success = response.Success };
                    }
                case UserRole.SuperAdmin:
                case UserRole.HealthCenterAdmin:
                case UserRole.LaboratoryAdmin:
                case UserRole.PharmacyAdmin:
                case UserRole.Reception:
                    {
                        UpdateAdminDTO updatedUser = _mapper.Map<UpdateAdminDTO>(user);
                        var response = await _adminService.UpdateAdmin((updatedUser as UpdateAdminDTO)!, user.Id!);
                        var verifiedUser = _mapper.Map<UsageUserDTO>(response.Data);

                        return new() { StatusCode = response.StatusCode, Message = response.Message, Data = verifiedUser, Errors = response.Errors, Success = response.Success };
                    }
                default:
                    return new() { StatusCode = 400, Errors = new[] { "Invalid Role" } };
            }
        }
        private async Task<SResponseDTO<string>> UpdatePassword(User user)
        {

            switch (user.Role)
            {
                case UserRole.Patient:
                    {
                        var response = await _patientService.UpdatePassword<Patient>(user.Id!, user.Password);
                        return new() { StatusCode = response.StatusCode, Message = response.Message, Errors = response.Errors, Success = response.Success };
                    }
                case UserRole.Doctor:
                    {
                        var response = await _doctorService.UpdatePassword<Doctor>(user.Id!, user.Password);

                        return new() { StatusCode = response.StatusCode, Message = response.Message, Errors = response.Errors, Success = response.Success };
                    }
                case UserRole.SuperAdmin:
                case UserRole.HealthCenterAdmin:
                case UserRole.LaboratoryAdmin:
                case UserRole.PharmacyAdmin:
                case UserRole.Reception:
                    {
                        var response = await _adminService.UpdatePassword<Administrator>(user.Id!, user.Password);

                        return new() { StatusCode = response.StatusCode, Message = response.Message, Errors = response.Errors, Success = response.Success };
                    }
                default:
                    return new() { StatusCode = 400, Errors = new[] { "Invalid Role" } };
            }
        }
    }
}