using Music.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Core.IServices
{
    public interface IAuthService
    {
        string GenerateJwtToken(string username, int userId, string[] roles);
        Task<UserWithTokenDTO> LoginAsync(UserDTO userDto);
        Task<UserWithTokenDTO> RegisterAsync(UserDTO userDto);
        Task<UserWithTokenDTO> UpdateAsync(int id, UserDTO userDto);
    }
}
