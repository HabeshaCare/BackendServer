using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Registration(RegistrationDTO model);
        Task<(int, string)> Login(LoginDTO model);
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}