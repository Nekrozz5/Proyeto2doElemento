using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Factura;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _service;
        private readonly IMapper _mapper;
        public FacturaController(FacturaService service, IMapper mapper)
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
        public async Task<IActionResult> Post([FromBody] FacturaCreateDTO dto)
        {
            var entity = _mapper.Map<Factura>(dto);
            await _service.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] FacturaUpdateDTO dto)
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
