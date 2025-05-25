using Microsoft.EntityFrameworkCore;
using Music.Core.DTOs;
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
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<User> _users;
        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _users = _dataContext.UserList;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _users.ToListAsync();
           // return await _users.Include(u=>u.Files).ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _users.Include("Files").FirstOrDefaultAsync(u=>u.Id == id);
            //return await (from u in _users.Include("Files") 
            //        where u.Id == id
            //        select u).FirstOrDefaultAsync();            
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<File>> GetFilesAsync(int id)
        {
            return await _users.Where(u => u.Id == id).SelectMany(u => u.Files).ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            var u = await _users.AddAsync(user);
            return u.Entity;
        }
        public async Task<User> UpdateAsync(int id, User user)
        {
            var userToUpdate = await _users.FindAsync(id);//בטוח לא null

            userToUpdate.Name = user.Name;
            userToUpdate.Email = user.Email;
            userToUpdate.Password = user.Password;
            userToUpdate.UpdatedAt = DateTime.Now;

            //productToUpdate.ProductName = !String.IsNullOrEmpty(product.ProductName) ? product.ProductName : productToUpdate.ProductName;

            //return _dataContext.SaveChange(dataProduct);

            return userToUpdate;
        }
        public async Task<User> DeleteAsync(int id)
        {
            var userToDelete = await _users.FindAsync(id);
            await Task.Run(() => _users.Remove(userToDelete));
            return userToDelete;
        }
    }
}
