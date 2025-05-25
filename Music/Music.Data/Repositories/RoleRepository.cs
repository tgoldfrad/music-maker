using Microsoft.EntityFrameworkCore;
using Music.Core.Entities;
using Music.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Data.Repositories
{
    public class RoleRepository:IRoleRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<Role> _roles;
        public RoleRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _roles = _dataContext.RoleList;
        }
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _roles.FindAsync(id);
        }
        public async Task<Role> GetByNameAsync(string name)
        {
            return await _roles.Where(r => r.Name.Equals(name)).FirstOrDefaultAsync();

        }


        public async Task<Role> AddAsync(Role role)
        {
            var f = await _roles.AddAsync(role);
            return f.Entity;
        }
        public async Task<Role> UpdateAsync(int id, Role role)
        {
            var roleToUpdate = await _roles.FindAsync(id);//בטוח לא null

            roleToUpdate.Name = role.Name;
            roleToUpdate.Description = role.Description;
            roleToUpdate.Permissions = role.Permissions;


            //productToUpdate.ProductName = !String.IsNullOrEmpty(product.ProductName) ? product.ProductName : productToUpdate.ProductName;

            //return _dataContext.SaveChange(dataProduct);

            return roleToUpdate;
        }
        public async Task<Role> DeleteAsync(int id)
        {
            var roleToDelete = await _roles.FindAsync(id);
            await Task.Run(() => _roles.Remove(roleToDelete));
            return roleToDelete;
        }
    }
}
