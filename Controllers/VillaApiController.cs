using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/VillaApi")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly ILogger<VillaApiController> _logger;
      
        private readonly IMapper _mapper;
        private readonly IVillaRepository _repo;

        public VillaApiController(ILogger<VillaApiController> logger, IMapper mapper, IVillaRepository repo)
        {
            _logger = logger;
            
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            var villas = await _repo.GetAllAsync();
            if (villas == null) { return NotFound(); } 
            return Ok(_mapper.Map<List<VillaDto>>(villas));
        }
        [Authorize]
        [HttpGet("{Id:int}", Name = "GetVilla")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VillaDto>> GetVilla(int Id)
        {
            _logger.LogInformation($"Getting the villa with Id as {Id}");
            if (Id <= 0) { return BadRequest(); }
            var villa = await _repo.GetAsync(u => u.Id == Id);
            if (villa == null) { return NotFound(0); }
            return Ok(_mapper.Map<VillaDto>(villa));
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaCreateDto>> CreateVilla([FromBody] VillaCreateDto createVillaDto)
        {
            if (createVillaDto == null) { return BadRequest(); }
            var villaModel = _mapper.Map<Villa>(createVillaDto);
            await _repo.CreateAsync(villaModel);
            return CreatedAtRoute("GetVilla", new { Id=villaModel.Id }, _mapper.Map<VillaCreateDto>(villaModel));
        }
        [Authorize(Roles = "admin")]

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int Id)
        {
            if (Id < 0) { return BadRequest(); }
            if (Id==0) { _logger.LogError($"Internal Server Error with Id given as {Id}"); return StatusCode(StatusCodes.Status500InternalServerError); }
            var villaModel = await _repo.GetAsync(u => u.Id == Id);
            await _repo.RemoveAsync(villaModel);
            return NoContent();
        }
        [Authorize(Roles = "CUSTOM")]

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateVilla(int Id, [FromBody] VillaUpdateDto villaData)
        {
            if (villaData == null || Id != villaData.Id |villaData.Id == 0) { return BadRequest(); }
            //var villa = _db.Villas.FirstOrDefault(u => u.Id == Id);
            var villaModel = _mapper.Map<Villa>(villaData);
            await _repo.UpdateAsync(villaModel);
            return NoContent();
        }
        [Authorize(Roles = "CUSTOM")]

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PatchVilla(int Id, [FromBody] JsonPatchDocument<VillaUpdateDto> patchData)
        {
            if (patchData == null || Id == 0) { return BadRequest(); }
            var villa = await _repo.GetAsync(u => u.Id == Id, tracked:false);
            if (villa == null) { return NotFound(); }
            var villaUpdateDto = _mapper.Map<VillaUpdateDto>(villa);
            
            patchData.ApplyTo(villaUpdateDto, ModelState);
            if (!ModelState.IsValid) { return StatusCode(StatusCodes.Status500InternalServerError); }
            var villaModel =_mapper.Map<Villa>(villaUpdateDto);
            await _repo.UpdateAsync(villaModel);
            return NoContent();
        }
    }
}
