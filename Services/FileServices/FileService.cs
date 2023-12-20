using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAuthentication.Services.FileServices
{
    public class FileService : IFileService
    {
        private List<string>_allowedFileTypes = new(){ ".pdf", ".jpg"};
        public async Task<(int, string, string?)> UploadFile(IFormFile file, string id, string uploadDir = "Uploads")
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException("Invalid file uploaded");

                var fileExtenstion = Path.GetExtension(file.FileName);
                if(!_allowedFileTypes.Contains(fileExtenstion))
                    throw new ArgumentException("Invalid file type");

                var fileName = (id ?? Guid.NewGuid().ToString()) + fileExtenstion;
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), uploadDir);

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(filestream);
                }

            var fileUrl = $"{uploadDir}/{fileName}";
            return (0, "File Created Successfully", fileUrl);

            }
            catch (Exception ex)
            {
                return (1, ex.Message, null);
            }
        }

        private 
    }
}