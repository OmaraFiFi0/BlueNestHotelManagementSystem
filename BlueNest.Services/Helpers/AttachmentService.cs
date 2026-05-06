using BlueNest.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Helpers
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        private readonly string[] _allowedExtensions = { ".png", ".jpg", ".svg", ".jpeg" };
        private readonly ILogger<AttachmentService> _logger;

        public AttachmentService(ILogger<AttachmentService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> UplodaFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file is null || file.Length == 0)
                    return null;

                if (file.Length > _maxFileSize)
                    return null;

                var extensions = Path.GetExtension(file.FileName).ToLower();

                if (!_allowedExtensions.Contains(extensions))
                    return null;

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() + extensions;

                var fullPath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(fullPath, FileMode.Create);

                await file.CopyToAsync(fileStream);


                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Unexpected Error Occurred While Uploding File");
                return null;
            }

        }

        public bool DeleteFile(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
                    return false;

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName, fileName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An Unexpected Error Occurred While Deleting File From Server");
                return false;
            }
        }
    }
}
