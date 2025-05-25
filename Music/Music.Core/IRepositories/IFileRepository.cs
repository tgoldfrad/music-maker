using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Music.Core.Entities.File;

namespace Music.Core.IRepositories
{
    public interface IFileRepository
    {
        Task<IEnumerable<File>> GetAllAsync();
        Task<IEnumerable<File>> GetFilesAsync();
        Task<File> GetByIdAsync(int id);
        Task<File> AddAsync(File file);
        Task<File> UpdateAsync(int id, File file);
        Task<File> DeleteAsync(int id);
    }
}
