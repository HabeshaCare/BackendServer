using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string?, UsageUserDTO?)> GetUserById(string id);
        Task<(int, string, UsageUserDTO?)> UpdateUser(UpdateUserDTO model, string userId);
        Task<(int, string, UsageUserDTO?)> UploadProfile(string userId, IFormFile? image);
    }
}