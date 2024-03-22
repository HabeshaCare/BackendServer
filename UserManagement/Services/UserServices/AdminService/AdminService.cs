using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.DTOs.HealthCenterDTOs;
using UserManagement.DTOs.LaboratoryDTOs;
using UserManagement.DTOs.PharmacyDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;
using UserManagement.Services.FileServices;
using UserManagement.Services.InstitutionService;
using UserManagement.Utils;

namespace UserManagement.Services.UserServices
{
    public class AdminService : UserService<Administrator>, IAdminService
    {
        private readonly IInstitutionService _institutionService;
        public AdminService(IOptions<MongoDBSettings> options, IFileService fileService, IMapper mapper, IInstitutionService institutionService) : base(options, fileService, mapper)
        {
            _institutionService = institutionService;
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

        public async Task<SResponseDTO<UsageAdminDTO>> UpdateVerification(bool verified, string id)
        {
            try
            {
                var response = await GetAdminById(id);
                UsageAdminDTO admin = response.Data!;
                UpdateAdminDTO adminDTO = new() { Verified = verified };
                var updateAdminTask = Task.Run(() => UpdateAdmin(adminDTO, id));

                if (!verified)
                {
                    Task updateInstitutionVerificationTask = admin.Role switch
                    {
                        UserRole.HealthCenterAdmin => Task.Run(() => _institutionService.UpdateInstitutionVerification<HealthCenterDTO>(admin.InstitutionId, false)),
                        UserRole.LaboratoryAdmin => Task.Run(() => _institutionService.UpdateInstitutionVerification<LaboratoryDTO>(admin.InstitutionId, false)),
                        UserRole.PharmacyAdmin => Task.Run(() => _institutionService.UpdateInstitutionVerification<PharmacyDTO>(admin.InstitutionId, false)),
                        _ => Task.Run(() => "Dummy task"),
                    };

                    await Task.WhenAll(updateAdminTask, updateInstitutionVerificationTask);
                }
                else
                {
                    await updateAdminTask;
                }

                var updatedAdmin = _mapper.Map<UsageAdminDTO>(updateAdminTask.Result.Data!);
                return new() { StatusCode = updateAdminTask.Result.StatusCode, Message = updateAdminTask.Result.Message, Data = updatedAdmin, Success = updateAdminTask.Result.Success, Errors = updateAdminTask.Result.Errors };
            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
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