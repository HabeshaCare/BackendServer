using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Services.InstitutionService
{
    public interface IInstitutionService
    {
        Task<(int, string?, USD?)> GetInstitutionById<USD>(string id);
        Task<(int, string, USD?)> UploadLicense<USD>(string institutionId, IFormFile? image);
        Task<(int, string, USD?)> UpdateInstitutionVerification<USD>(string institutionId, bool status);
    }
}