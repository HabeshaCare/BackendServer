using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;

namespace UserManagement.Services.InstitutionService
{
    public interface IInstitutionService
    {
        Task<SResponseDTO<USD>> GetInstitutionById<USD>(string id);
        Task<SResponseDTO<USD>> UploadLicense<USD>(string institutionId, IFormFile? image);
        Task<SResponseDTO<USD>> UpdateInstitutionVerification<USD>(string institutionId, bool status);
    }
}