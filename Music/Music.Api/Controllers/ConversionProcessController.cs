using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;
using Music.Core.Entities;
using Music.Core.IServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Music.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversionProcessController : ControllerBase
    {
        private readonly IConversionProcessService _conversionProcessService;
        private readonly IMapper _mapper;
        public ConversionProcessController(IConversionProcessService conversionProcessService, IMapper mapper)
        {
            _conversionProcessService = conversionProcessService;
            _mapper = mapper;
        }
        // GET: api/<ConversionProcessController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConversionProcessDTO>>> Get()
        {
            return Ok(await _conversionProcessService.GetAllAsync());
        }

        // GET api/<ConversionProcessController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConversionProcessDTO>> Get(int id)
        {
            if (id < 0)
                return BadRequest();
            var conversionProcess = await _conversionProcessService.GetByIdAsync(id);
            if (conversionProcess == null)
            {
                return NotFound();
            }
            return conversionProcess;
        }

        // POST api/<ConversionProcessController>
        [HttpPost]
        public async Task<ActionResult<ConversionProcessDTO>> Post([FromBody] ConversionProcessPostModel conversionProcess)
        {
            var conversionProcessDto = _mapper.Map<ConversionProcessDTO>(conversionProcess);
            conversionProcessDto = await _conversionProcessService.AddAsync(conversionProcessDto);
            if (conversionProcessDto != null)
                return conversionProcessDto;
            return BadRequest();
        }

        // PUT api/<ConversionProcessController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ConversionProcessDTO>> Put(int id, [FromBody] ConversionProcessPostModel conversionProcess)
        {
            if (id < 0)
                return BadRequest();
            var conversionProcessDto = _mapper.Map<ConversionProcessDTO>(conversionProcess);
            conversionProcessDto = await _conversionProcessService.UpdateAsync(id, conversionProcessDto);
            if (conversionProcessDto != null)
                return conversionProcessDto;
            return NotFound();
        }

        // DELETE api/<ConversionProcessController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ConversionProcessDTO>> Delete(int id)
        {
            if (id < 0)
                return BadRequest();
            var conversionProcessDto = await _conversionProcessService.DeleteAsync(id);
            if (conversionProcessDto != null)
                return conversionProcessDto;
            return NotFound();
        }
    }
}
