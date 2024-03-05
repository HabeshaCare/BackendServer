using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string, USD?)> UploadProfilePic<USD>(string userId, IFormFile? image);
    }
}