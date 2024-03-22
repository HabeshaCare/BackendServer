using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;

namespace UserManagement.Services.FileServices
{
    public interface IFileService
    {
        Task<SResponseDTO<string>> UploadFile(IFormFile file, string id, string uploadDir);
    }
}