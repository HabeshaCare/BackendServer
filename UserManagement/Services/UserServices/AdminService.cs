using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class AdminService : UserService<Administrator>, IAdminService
    {
        public AdminService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<(int, string, UsageAdminDTO?)> AddAdmin(Administrator admin)
        {
            return await AddUser<UsageAdminDTO>(admin);
        }

        public async Task<(int, string?, UsageAdminDTO?)> GetAdminByEmail(string adminEmail)
        {
            return await GetUserByEmail<UsageAdminDTO>(adminEmail);
        }

        public async Task<(int, string?, UsageAdminDTO?)> GetAdminById(string adminId)
        {
            return await GetUserById<UsageAdminDTO>(adminId);
        }

        public async Task<(int, string, UsageAdminDTO?)> UpdateAdmin(UpdateAdminDTO adminDTO, string id)
        {
            return await UpdateUser<UpdateAdminDTO, UsageAdminDTO>(adminDTO, id);
        }
    }
}