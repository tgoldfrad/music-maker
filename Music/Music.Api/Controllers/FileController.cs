using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IServices;
using Music.Service;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IAWSService _awsService;

        public FileController(IFileService fileService, IMapper mapper, IAWSService awsService)
        {
            _fileService = fileService;
            _mapper = mapper;
            _awsService = awsService;
        }
        // GET: api/<FileController>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<FileDTO>>> Get()
        {
            return Ok(await _fileService.GetAllAsync());
        }

        [HttpGet("share-files")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FileDTO>>> GetFiles()
        {
            var files = await _fileService.GetFilesAsync();
            if (files == null)
            {
                return NotFound();
            }
            return Ok(files);
        }

        // GET api/<FileController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<FileDTO>> Get(int id)
        {
            if (id < 0)
                return BadRequest();
            var file = await _fileService.GetByIdAsync(id);
            if (file == null)
            {
                return NotFound();
            }
            return file;
        }

        // POST api/<FileController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FileDTO>> Post([FromBody] FilePostModel file)
        {
            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //var handler = new JwtSecurityTokenHandler();
            //var jwtToken = handler.ReadJwtToken(token);
            //var userIdClaim = jwtToken.Claims.FirstOrDefault(c=>c.Type=="userId");
            //if(userIdClaim == null)
            //    return Unauthorized();
            //var userId = userIdClaim.Value;

            if (file == null)
                return BadRequest("No file Provided");
            var fileDto = _mapper.Map<FileDTO>(file);
            //fileDto.CreatedBy = int.Parse(userId);

            try
            {
                var result = await _fileService.AddAsync(fileDto);
                return result;
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid file data.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT api/<FileController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<FileDTO>> Put(int id, [FromBody] FilePutModel file)
        {
            if (id < 0)
                return BadRequest();
            var fileDto = _mapper.Map<FileDTO>(file);
            try
            {
                var result = await _fileService.UpdateAsync(id, fileDto);
                return result;
            }
            catch (KeyNotFoundException)
            {
                return NotFound("File not found");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid file data.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/<FileController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<FileDTO>> Delete(int id)
        {
            if (id < 0)
                return BadRequest();
            try
            {
                var userDto = await _fileService.DeleteAsync(id);
                return userDto;
            }
            catch (KeyNotFoundException)
            {
                return NotFound("File not found");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("presigned-url")]
        [Authorize]
        public async Task<ActionResult<string>> GetPreSignedUrl([FromQuery] int userId, [FromQuery] string fileName, [FromQuery] string contentType)
        {
            if(string.IsNullOrEmpty(fileName)||string.IsNullOrEmpty(contentType))
                return BadRequest("Invalid file data");

            var url = await _awsService.GetPreSignedUrlAsync(userId,fileName,contentType);
            return Ok(new { url });
         }

        [HttpGet("download-url")]
        [Authorize]
        public async Task<ActionResult<string>> GetDownladUrl([FromQuery] int userId, [FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Invalid file data");

            var url = await _awsService.GetDownloadUrlAsync(userId, fileName);
            return Ok(new { url });
        }
        
        // POST api/<FileController>
        [HttpPost("convert")]
        //[Authorize]
        public async Task<ActionResult> ConvertPost([FromBody] FilePostModel file)
        {
            byte[] wavBytes = await _fileService.ConvertToSong(file.Name,file.Path);
            return File(wavBytes, "audio/wav", "output.wav");
        }
    }
}
//return Ok(new { wavBytes });
