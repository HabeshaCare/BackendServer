using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Services
{
    public interface IAuthService
    {
        Task<(int, string, UsageUserDTO?)> Registration(RegistrationDTO model);
        Task<(int, string, UsageUserDTO?)> Login(LoginDTO model);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}