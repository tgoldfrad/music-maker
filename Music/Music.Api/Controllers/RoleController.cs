using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;
using Music.Core.IServices;
using Music.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }
        // GET: api/<RoleController>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> Get()
        {
            return Ok(await _roleService.GetAllAsync());
        }

        // GET api/<RoleController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<RoleDTO>> Get(int id)
        {
            if (id < 0)
                return BadRequest();
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return role;
        }
        [HttpGet("name/{id}")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<ActionResult<RoleDTO>> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();
            var role = await _roleService.GetByNameAsync(name);
            if (role == null)
            {
                return NotFound();
            }
            return role;
        }
        // POST api/<RoleController>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]

        public async Task<ActionResult<RoleDTO>> Post([FromBody] RolePostModel role)
        {
            var roleDto = _mapper.Map<RoleDTO>(role);
            roleDto = await _roleService.AddAsync(roleDto);
            if (roleDto != null)
                return roleDto;
            return BadRequest();
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<ActionResult<RoleDTO>> Put(int id, [FromBody] RolePostModel role)
        {
            if (id < 0)
                return BadRequest();
            var roleDto = _mapper.Map<RoleDTO>(role);
            roleDto = await _roleService.UpdateAsync(id, roleDto);
            if (roleDto != null)
                return roleDto;
            return NotFound();
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<RoleDTO>> Delete(int id)
        {
            if (id < 0)
                return BadRequest();
            var roleDto = await _roleService.DeleteAsync(id);
            if (roleDto != null)
                return roleDto;
            return NotFound();
        }
    }
}
