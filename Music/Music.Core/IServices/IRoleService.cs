using Music.Core.DTOs;
using Music.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllAsync();
        Task<RoleDTO> GetByIdAsync(int id);
        Task<RoleDTO> GetByNameAsync(string name);

        Task<RoleDTO> AddAsync(RoleDTO role);
        Task<RoleDTO> UpdateAsync(int id, RoleDTO role);
        Task<RoleDTO> DeleteAsync(int id);
    }
}
