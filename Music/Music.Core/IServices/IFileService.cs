using Music.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IFileService
    {
        Task<IEnumerable<FileDTO>> GetAllAsync();
        Task<IEnumerable<FileDTO>> GetFilesAsync();
        Task<FileDTO> GetByIdAsync(int id);
        Task<FileDTO> AddAsync(FileDTO file);
        Task<FileDTO> UpdateAsync(int id, FileDTO file);
        Task<FileDTO> DeleteAsync(int id);
        Task<byte[]> ConvertToSong(string fileName,string path);
    }
}
