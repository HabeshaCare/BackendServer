using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string?, USD?)> GetUserById<USD>(string userId);
        Task<(int, string?, USD?)> GetUserByEmail<USD>(string email);
        Task<(int, string, USD?)> UploadProfilePic<USD>(string userId, IFormFile? image);
    }
}