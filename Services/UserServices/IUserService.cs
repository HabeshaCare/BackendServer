using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAuthentication.Models.DTOs.UserDTOs;

namespace UserAuthentication.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string?, UsageUserDTO?)> GetUserById(string id);
        Task<(int, string, UsageUserDTO?)> UpdateUser(UpdateUserDTO model, string userId);
        Task<(int, string, UsageUserDTO?)> UploadProfile(string userId, IFormFile? image);
    }
}