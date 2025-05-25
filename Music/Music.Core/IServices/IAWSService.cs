using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IAWSService
    {
        Task<string> GetPreSignedUrlAsync(int userId, string fileName, string contentType);
        Task<string> GetDownloadUrlAsync(int userId, string fileName);
    }
}
