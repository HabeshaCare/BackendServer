using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.DTOs;

namespace UserManagement.Services.FileServices
{
    public class FileService : IFileService
    {
        private List<string> _allowedFileTypes = new() { ".pdf", ".jpg" };

        /// Uploads a file to the specified directory.
        public async Task<SResponseDTO<string>> UploadFile(IFormFile file, string id, string uploadDir = "Uploads")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new() { StatusCode = 400, Errors = new[] { "File is empty" } };

                var fileExtension = Path.GetExtension(file.FileName);
                if (!_allowedFileTypes.Contains(fileExtension))
                    return new() { StatusCode = 400, Errors = new[] { "Invalid file type" } };

                var fileName = $"{id}_{Guid.NewGuid()}{fileExtension}";
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), uploadDir);

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var fileUrl = $"{uploadDir}/{fileName}";
                return new() { StatusCode = 201, Message = "File Created Successfully", Data = fileUrl, Success = true };

            }
            catch (Exception ex)
            {
                return new() { StatusCode = 500, Errors = new[] { ex.Message } };
            }
        }
    }
}