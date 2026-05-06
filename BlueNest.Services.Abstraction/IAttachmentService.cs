using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueNest.Services.Abstraction
{
    public interface IAttachmentService
    {

        Task<string?> UplodaFileAsync(IFormFile file, string folderName);

        bool DeleteFile(string fileName, string folderName);

    }
}
