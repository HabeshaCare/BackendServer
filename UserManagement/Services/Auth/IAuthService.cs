using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Models.DTOs;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services
{
    public interface IAuthService
    {
        Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model);
        Task<(int, string, UsageUserDTO?)> Login(LoginDTO model);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}