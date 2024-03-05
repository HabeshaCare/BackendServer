using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models.DTOs.UserDTOs;

namespace UserManagement.Services.UserServices
{
    public interface IUserService
    {
        Task<(int, string?, USD?)> GetUserById<USD>(string id);
        Task<(int, string?, USD?)> GetUserByEmail<USD>(string doctorEmail);
        Task<(int, string, USD?)> UpdateUser<UD, USD>(UD model, string userId);
        Task<(int, string, USD?)> UploadProfile<USD>(string userId, IFormFile? image);
    }
}