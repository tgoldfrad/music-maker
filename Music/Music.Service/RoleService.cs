using AutoMapper;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IRepositories;
using Music.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Service
{
    public class RoleService:IRoleService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public RoleService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<IEnumerable<RoleDTO>> GetAllAsync()
        {
            return _mapper.Map<List<RoleDTO>>(await _repositoryManager.Roles.GetAllAsync());
        }
        public async Task<RoleDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<RoleDTO>(await _repositoryManager.Roles.GetByIdAsync(id));
        }
        public async Task<RoleDTO> GetByNameAsync(string name)
        {
            return _mapper.Map<RoleDTO>(await _repositoryManager.Roles.GetByNameAsync(name));
        }

        public async Task<RoleDTO> AddAsync(RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            roleDto = _mapper.Map<RoleDTO>(await _repositoryManager.Roles.AddAsync(role));
            await _repositoryManager.SaveAsync();
            return roleDto;
        }
        public async Task<RoleDTO> UpdateAsync(int id, RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            roleDto = _mapper.Map<RoleDTO>(await _repositoryManager.Roles.UpdateAsync(id, role));
            await _repositoryManager.SaveAsync();
            return roleDto;
        }
        public async Task<RoleDTO> DeleteAsync(int id)
        {
            var roleDto = _mapper.Map<RoleDTO>(await _repositoryManager.Roles.DeleteAsync(id));
            await _repositoryManager.SaveAsync();
            return roleDto;
        }
    }
}
