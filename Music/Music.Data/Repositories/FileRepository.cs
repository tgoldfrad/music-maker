using Microsoft.EntityFrameworkCore;
using Music.Core.Entities;
using Music.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Music.Core.Entities.File;

namespace Music.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<File> _files;
        public FileRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _files = _dataContext.FileList;
        }
        public async Task<IEnumerable<File>> GetAllAsync()
        {
            return await _files.ToListAsync();
        }
        public async Task<IEnumerable<File>> GetFilesAsync()
        {
            return await _files.Where(f => f.IsUser).ToListAsync();
        }
        public async Task<File> GetByIdAsync(int id)
        {
            return await _files.FindAsync(id);
        }

        public async Task<File> AddAsync(File file)
        {
            var f = await _files.AddAsync(file);
            return f.Entity;
        }
        public async Task<File> UpdateAsync(int id, File file)
        {
            var fileToUpdate = await _files.FindAsync(id);//בטוח לא null

            fileToUpdate.Path = file.Path;
            fileToUpdate.Name = file.Name;
            fileToUpdate.IsUser = file.IsUser;
            fileToUpdate.Size = file.Size;
            fileToUpdate.UpdatedAt = DateTime.Now;

            //productToUpdate.ProductName = !String.IsNullOrEmpty(product.ProductName) ? product.ProductName : productToUpdate.ProductName;

            //return _dataContext.SaveChange(dataProduct);

            return fileToUpdate;
        }
        public async Task<File> DeleteAsync(int id)
        {
            var fileToDelete = await _files.FindAsync(id);
            await Task.Run(() => _files.Remove(fileToDelete));
            return fileToDelete;
        }
    }
}
