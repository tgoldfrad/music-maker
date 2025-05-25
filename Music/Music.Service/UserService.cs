using AutoMapper;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IRepositories;
using Music.Core.IServices;
//using Music.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Service
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public UserService(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            return _mapper.Map<List<UserDTO>>(await _repositoryManager.Users.GetAllAsync());
        }
        public async Task<UserDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<UserDTO>(await _repositoryManager.Users.GetByIdAsync(id));
        }
        public async Task<UserDTO> GetByEmailAsync(string email)
        {
            return _mapper.Map<UserDTO>(await _repositoryManager.Users.GetByEmailAsync(email));
        }
        public async Task<IEnumerable<FileDTO>> GetFilesAsync(int id)
        {
            var files = await _repositoryManager.Users.GetFilesAsync(id);
            var filesDto = _mapper.Map<List<FileDTO>>(files);
            return filesDto;
        }

        //public async Task<UserDTO> AddAsync(UserDTO userDto)
        //{
        //    var user = _mapper.Map<User>(userDto);
        //    userDto = _mapper.Map<UserDTO>(await _repositoryManager.Users.AddAsync(user));
        //    await _repositoryManager.SaveAsync();
        //    return userDto;
        //}
        //public async Task<UserDTO> UpdateAsync(int id, UserDTO userDto)
        //{
        //    var user = _mapper.Map<User>(userDto);
        //    userDto = _mapper.Map<UserDTO>(await _repositoryManager.Users.UpdateAsync(id, user));
        //    await _repositoryManager.SaveAsync();
        //    return userDto;
        //}
        public async Task<UserDTO> DeleteAsync(int id)
        {
            var u = await _repositoryManager.Users.GetByIdAsync(id);
            if (u == null)
                throw new KeyNotFoundException();
            var userDto = _mapper.Map<UserDTO>(await _repositoryManager.Users.DeleteAsync(id));
            await _repositoryManager.SaveAsync();
            return userDto;
        }
        public async Task<IEnumerable<MonthlyStatsDto>> GetRegistrationStatsAsync()
        {
            var users = await _repositoryManager.Users.GetAllAsync();
            var startDate = DateTime.Now.AddMonths(-12);

            var registrations = users
                .Where(u => u.CreatedAt >= startDate)
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new MonthlyStatsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            return registrations;
        }
    }
}
