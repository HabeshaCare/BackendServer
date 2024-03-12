using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.UserDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services
{
    public interface IAuthService
    {
        Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model);
        Task<(int, string, UsageUserDTO?)> Login(LoginDTO model);
        Task<(int, string, UsageUserDTO?)> VerifyEmail(string token);
        Task<(int, string, UsageUserDTO?)> ForgotPassword(string email);
        Task<(int, string)> ResetPassword(UserResetPasswordDTO request);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}