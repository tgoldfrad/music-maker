using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;
using Music.Core.IServices;
using Music.Service;
using System.IdentityModel.Tokens.Jwt;
//using Music.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;


        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        [HttpPost("login")]
        //[Authorize]
        public async Task<ActionResult<UserWithTokenDTO>> Login([FromBody] LoginModel value)
        {
            if (value == null)
                return BadRequest("No value Provided");
            var userDto = _mapper.Map<UserDTO>(value);
            try
            {
                var result = await _authService.LoginAsync(userDto);
                return result;
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid password");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserWithTokenDTO>> Register([FromBody] UserPostModel value)
        {
            if (value == null)
                return BadRequest("No value Provided");
            var userDto = _mapper.Map<UserDTO>(value);
            try
            {
                var result = await _authService.RegisterAsync(userDto);
                return result;
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid user data.");
            }
            catch (InvalidOperationException)
            {
                return Conflict("Email already exist");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserWithTokenDTO>> Put(int id, [FromBody] UserPutModel user)
        {
            if (id < 0)
                return BadRequest();
            var userDto = _mapper.Map<UserDTO>(user);

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null)
                return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            if (userId != id)
                return Forbid("Can not update other user");
            try
            {
                var result = await _authService.UpdateAsync(id, userDto);
                return result;
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (InvalidOperationException)
            {
                return Conflict("Email already exist");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid user data.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
