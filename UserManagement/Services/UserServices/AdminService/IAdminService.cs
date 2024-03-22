using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;
using UserManagement.DTOs.AdminDTOs;
using UserManagement.Models;
using UserManagement.Models.DTOs.OptionsDTO;

namespace UserManagement.Services.UserServices
{
    public interface IAdminService : IUserService
    {
        Task<SResponseDTO<UsageAdminDTO[]>> GetAdmins(FilterDTO filterOptions, int page, int size);
        Task<SResponseDTO<UsageAdminDTO>> GetAdminById(string adminId);
        Task<SResponseDTO<UsageAdminDTO>> GetAdminByEmail(string adminEmail);
        Task<SResponseDTO<Administrator>> AddAdmin(Administrator admin);
        Task<SResponseDTO<Administrator>> UpdateAdmin(UpdateAdminDTO adminDTO, string id);
        Task<SResponseDTO<UsageAdminDTO>> UpdateVerification(bool verified, string id);
        Task<SResponseDTO<bool>> AddInstitutionAccess(string adminId, string institutionId);
        Task<SResponseDTO<bool>> RemoveInstitutionAccess(string adminId);

    }
}