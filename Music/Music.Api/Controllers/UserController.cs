using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IServices;
using Music.Service;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        // GET: api/<UserController>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            return Ok(await _userService.GetAllAsync());
        }


        // GET api/<UserController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            if (id < 0)
                return BadRequest();
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Get(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest();
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpGet("{id}/files")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FileDTO>>> GetFiles(int id)
        {
            if (id<0)
                return BadRequest();
            var files = await _userService.GetFilesAsync(id);
            if (files == null)
            {
                return NotFound();
            }
            return Ok(files);
        }

        //// POST api/<UserController>
        //[HttpPost]
        //public async Task<ActionResult<UserDTO>> Post([FromBody] UserPostModel user)
        //{
        //    var userDto = _mapper.Map<UserDTO>(user);
        //    userDto = await _userService.AddAsync(userDto);
        //    if (userDto != null)
        //        return userDto;
        //    return BadRequest();
        //}

        //// PUT api/<UserController>/5
        //[HttpPut("{id}")]
        //public async Task<ActionResult<UserDTO>> Put(int id, [FromBody] UserPutModel user)
        //{
        //    if (id <= 0)
        //        return BadRequest();
        //    var userDto = _mapper.Map<UserDTO>(user);
        //    userDto = await _userService.UpdateAsync(id, userDto);
        //    if (userDto != null)
        //        return userDto;
        //    return NotFound();
        //}

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UserDTO>> Delete(int id)
        {
            if (id < 0)
                return BadRequest();
            try
            {
                var userDto = await _userService.DeleteAsync(id);
                return userDto;
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("registration-stats")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<ActionResult<IEnumerable<MonthlyStatsDto>>> GetRegistrationStats()
        {
            return Ok(await _userService.GetRegistrationStatsAsync());
        }
    }
}
