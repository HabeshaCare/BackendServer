using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;

namespace UserManagement.Services.UserServices
{
    public interface IAdminService : IUserService
    {
        Task<(int, string?, UsageAdminDTO?)> GetAdminById(string adminId);
        Task<(int, string?, UsageAdminDTO?)> GetAdminByEmail(string adminEmail);
        Task<(int, string, Administrator?)> AddAdmin(Administrator admin);
        Task<(int, string, Administrator?)> UpdateAdmin(UpdateAdminDTO adminDTO, string id);
    }
}