using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

        public async Task<(int, string, Administrator?)> AddAdmin(Administrator admin)
        {
            return await AddUser<Administrator>(admin);
        }

        public async Task<(int, string?, UsageAdminDTO?)> GetAdminByEmail(string adminEmail)
        {
            return await GetUserByEmail<UsageAdminDTO>(adminEmail);
        }

        public async Task<(int, string?, UsageAdminDTO?)> GetAdminById(string adminId)
        {
            return await GetUserById<UsageAdminDTO>(adminId);
        }

        public async Task<(int, string, Administrator?)> UpdateAdmin(UpdateAdminDTO adminDTO, string id)
        {
            return await UpdateUser<UpdateAdminDTO, Administrator>(adminDTO, id);
        }
        public async Task<(int, string, UsageAdminDTO?)> AddInstitutionAccess(string adminId, string institutionId)
        {
            try
            {
                var filter = Builders<Administrator>.Filter.Eq(a => a.Id, adminId);
                var update = Builders<Administrator>.Update.Push(a => a.InstitutionsId, institutionId);
                var result = await _collection.UpdateOneAsync(filter, update);
                if (result.ModifiedCount > 0)
                {
                    return (1, "Institution access added successfully", null);
                }
                else
                {
                    return (0, "Failed to add institution access", null);
                }

            }
            catch (Exception ex)
            {

                return (0, ex.Message, null);
            }

        }
    }
}