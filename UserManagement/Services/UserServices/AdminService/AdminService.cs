using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class AdminService : UserService<Administrator>, IAdminService
    {
        public AdminService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper) : base(options, fileService, mapper)
        {
        }

        public async Task<SResponseDTO<Administrator>> AddAdmin(Administrator admin)
        {
            return await AddUser<Administrator>(admin);
        }

        public async Task<SResponseDTO<UsageAdminDTO[]>> GetAdmins(FilterDTO filterOptions, int page, int size)
        {
            var filterDefinition = PrepareFilterDefinition(filterOptions);

            return await GetUsers<UsageAdminDTO>(filterDefinition, page, size);
        }

        public async Task<SResponseDTO<UsageAdminDTO>> GetAdminByEmail(string adminEmail)
        {
            return await GetUserByEmail<UsageAdminDTO>(adminEmail);
        }

        public async Task<SResponseDTO<UsageAdminDTO>> GetAdminById(string adminId)
        {
            return await GetUserById<UsageAdminDTO>(adminId);
        }

        public async Task<SResponseDTO<Administrator>> UpdateAdmin(UpdateAdminDTO adminDTO, string id)
        {
            return await UpdateUser<UpdateAdminDTO, Administrator>(adminDTO, id);
        }

        public async Task<SResponseDTO<bool>> AddInstitutionAccess(string adminId, string institutionId)
        {
            try
            {
                var filter = Builders<Administrator>.Filter.Eq(a => a.Id, adminId);
                var update = Builders<Administrator>.Update.Set(a => a.InstitutionId, institutionId);
                var result = await _collection.UpdateOneAsync(filter, update);

                bool success = result.ModifiedCount > 0;

                if (success)
                    return new() { StatusCode = 200, Message = "Access granted", Success = true };
                else
                    return new() { StatusCode = 404, Errors = new[] { "Failed to grant access to admin" } };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }

        public async Task<SResponseDTO<bool>> RemoveInstitutionAccess(string adminId)
        {
            try
            {
                var filter = Builders<Administrator>.Filter.Eq(a => a.Id, adminId);
                var update = Builders<Administrator>.Update.Set(a => a.InstitutionId, "");
                var result = await _collection.UpdateOneAsync(filter, update);

                bool success = result.ModifiedCount > 0;

                if (success)
                    return new() { StatusCode = 200, Message = "Access revoked", Success = true };
                else
                    return new() { StatusCode = 404, Errors = new[] { "Failed to revoke access from admin" } };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }
    }
}