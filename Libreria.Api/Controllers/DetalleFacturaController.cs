using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.DetalleFactura;
using Libreria.Infrastructure.DTOs.Factura;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly DetalleFacturaService _service;
        private readonly IMapper _mapper;
        public DetalleFacturaController(DetalleFacturaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            return entity is null ? NotFound() : Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DetalleFacturaCreateDTO dto)
        {
            var entity = _mapper.Map<DetalleFactura>(dto);
            await _service.AddAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
