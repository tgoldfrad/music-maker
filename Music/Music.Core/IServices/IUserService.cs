using Music.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllAsync();
        Task<UserDTO> GetByIdAsync(int id);
        Task<UserDTO> GetByEmailAsync(string email);
        Task<IEnumerable<FileDTO>> GetFilesAsync(int id);
        //Task<UserDTO> AddAsync(UserDTO user);
        //Task<UserDTO> UpdateAsync(int id, UserDTO user);
        Task<UserDTO> DeleteAsync(int id);
        Task<IEnumerable<MonthlyStatsDto>> GetRegistrationStatsAsync();
    }
}
