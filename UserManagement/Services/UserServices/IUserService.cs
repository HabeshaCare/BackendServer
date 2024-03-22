using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;

namespace UserManagement.Services.UserServices
{
    public interface IUserService
    {
        Task<SResponseDTO<USD>> GetUserById<USD>(string userId);
        Task<SResponseDTO<USD>> GetUserByEmail<USD>(string email);
        Task<SResponseDTO<USD>> GetUserByVerificationToken<USD>(string token);
        Task<SResponseDTO<USD>> GetUserByResetToken<USD>(string token);
        Task<SResponseDTO<USD>> UploadProfilePic<USD>(string userId, IFormFile? image);
        Task<SResponseDTO<string>> UpdatePassword<USD>(string id, string newHashedPassword);
    }
}