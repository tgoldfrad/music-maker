using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IRepositories;
using Music.Core.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Music.Service
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public AuthService(IConfiguration configuration, IRepositoryManager repositoryManager, IMapper mapper)
        {
            _configuration = configuration;
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public string GenerateJwtToken(string username,int userId, string[] roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, username),
            new Claim("userId", userId.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserWithTokenDTO> LoginAsync(UserDTO userDto)
        {
            var user = await _repositoryManager.Users.GetByEmailAsync(userDto.Email);
            if (user == null)
                throw new KeyNotFoundException();
            if (!user.Password.Equals(userDto.Password))
                throw new UnauthorizedAccessException();
            var role = await _repositoryManager.Roles.GetByIdAsync(user.RoleId);
            string token = GenerateJwtToken(user.Name, user.Id, [role.Name]);
            userDto = _mapper.Map<UserDTO>(user);
            return new UserWithTokenDTO { UserDto = userDto, Token = token };
        }
        public async Task<UserWithTokenDTO> RegisterAsync(UserDTO userDto)
        {
            var role = await _repositoryManager.Roles.GetByNameAsync(userDto.Role);
            if (role == null || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
                throw new ArgumentException();
            var userByEmail = await _repositoryManager.Users.GetByEmailAsync(userDto.Email);
            if (userByEmail != null)
                throw new InvalidOperationException();
            var user = _mapper.Map<User>(userDto);
            user.Role = role;
            user = await _repositoryManager.Users.AddAsync(user);
            await _repositoryManager.SaveAsync();
            userDto = _mapper.Map<UserDTO>(user);
            string token = GenerateJwtToken(userDto.Name,userDto.Id,[userDto.Role ]);
            return new UserWithTokenDTO { UserDto = userDto, Token = token };

        }
        public async Task<UserWithTokenDTO> UpdateAsync(int id, UserDTO userDto)
        {
            var u = await _repositoryManager.Users.GetByIdAsync(id);
            if(u==null)
                throw new KeyNotFoundException();

            var userByEmail = await _repositoryManager.Users.GetByEmailAsync(userDto.Email);
            if (userByEmail == null && userByEmail.Id != id)
                throw new InvalidOperationException();

            var role = await _repositoryManager.Roles.GetByIdAsync(u.RoleId);
            if (role == null || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
                throw new ArgumentException(); 
            var user = _mapper.Map<User>(userDto);
            user.Role = role;
            userDto = _mapper.Map<UserDTO>(await _repositoryManager.Users.UpdateAsync(id, user));
            await _repositoryManager.SaveAsync();
            userDto = _mapper.Map<UserDTO>(user);
            string token = GenerateJwtToken(userDto.Name, userDto.Id, [userDto.Role]);
            return new UserWithTokenDTO { UserDto = userDto, Token = token };
        }
    }
}
