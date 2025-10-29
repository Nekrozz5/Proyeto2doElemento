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
        private readonly FacturaService _facturaService;
        private readonly IMapper _mapper;

        public FacturaController(FacturaService facturaService, IMapper mapper)
        {
            _facturaService = facturaService;
            _mapper = mapper;
        }

        // ✅ Obtener todas las facturas (con cliente y detalles)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetAll()
        {
            var facturas = await _facturaService.GetAllAsync();
            var facturasDto = _mapper.Map<IEnumerable<FacturaDTO>>(facturas);
            return Ok(facturasDto);
        }

        // ✅ Obtener una factura por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDTO>> GetById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            if (factura == null)
                return NotFound();

            var facturaDto = _mapper.Map<FacturaDTO>(factura);
            return Ok(facturaDto);
        }

        // ✅ Crear una nueva factura (guarda cliente + detalles + total)
        [HttpPost]
        public async Task<ActionResult> Create(FacturaCreateDTO dto)
        {
            try
            {
                var factura = _mapper.Map<Factura>(dto);
                await _facturaService.AddAsync(factura);

                var facturaDto = _mapper.Map<FacturaDTO>(factura);
                return CreatedAtAction(nameof(GetById), new { id = factura.Id }, facturaDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Eliminar una factura
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _facturaService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
