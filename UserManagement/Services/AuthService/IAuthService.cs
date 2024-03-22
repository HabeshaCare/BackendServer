using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services
{
    public interface IAuthService
    {
        Task<SResponseDTO<UsageUserDTO>> Registration(RegistrationDTO model);
        Task<SResponseDTO<UsageUserDTO>> Login(LoginDTO model);
        Task<SResponseDTO<UsageUserDTO>> VerifyEmail(string token);
        Task<SResponseDTO<UsageUserDTO>> ForgotPassword(string email);
        Task<SResponseDTO<string>> ResetPassword(UserResetPasswordDTO request);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}