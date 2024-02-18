using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Services.FileServices
{
    public interface IFileService
    {
        Task<(int, string, string?)> UploadFile(IFormFile file, string id, string uploadDir);
    }
}