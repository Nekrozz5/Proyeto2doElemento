using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Libro;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController : ControllerBase
    {
        private readonly LibroService _service;
        private readonly IMapper _mapper;
        public LibroController(LibroService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_service.GetAll());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LibroCreateDto dto)
        {
            var entity = _mapper.Map<Libro>(dto);
            await _service.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] LibroUpdateDto dto)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();
            _mapper.Map(dto, existing);
            await _service.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
