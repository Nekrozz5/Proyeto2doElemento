using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
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

        // ========================================
        // GET: Todas las facturas
        // ========================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var facturas = _facturaService.GetAll();
            var facturasDto = _mapper.Map<IEnumerable<FacturaDTO>>(facturas);
            return Ok(facturasDto);
        }

        // ========================================
        // GET: Factura por ID
        // ========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            if (factura == null)
                return NotFound();

            var facturaDto = _mapper.Map<FacturaDTO>(factura);
            return Ok(facturaDto);
        }

        // ========================================
        // POST: Crear una nueva factura
        // ========================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FacturaCreateDTO dto)
        {
            // Mapeamos el DTO a la entidad Factura
            var factura = _mapper.Map<Factura>(dto);
            await _facturaService.AddAsync(factura);

            // Recuperamos la factura ya guardada con includes
            var facturaCompleta = await _facturaService.GetByIdAsync(factura.Id);

            // Mapeamos nuevamente al DTO para devolver solo los datos necesarios
            var facturaDto = _mapper.Map<FacturaDTO>(facturaCompleta);

            return Ok(facturaDto);
        }

        // ========================================
        // PUT: Actualizar una factura existente
        // ========================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Factura factura)
        {
            if (id != factura.Id)
                return BadRequest("El ID de la factura no coincide.");

            _facturaService.Update(factura);
            return Ok("Factura actualizada correctamente.");
        }

        // ========================================
        // DELETE: Eliminar una factura
        // ========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _facturaService.DeleteAsync(id);
            return Ok("Factura eliminada correctamente.");

        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] FacturaQueryFilter filters)
        {
            var facturas = await _facturaService.GetFilteredAsync(filters);
            return Ok(facturas);
        }
    }
}
