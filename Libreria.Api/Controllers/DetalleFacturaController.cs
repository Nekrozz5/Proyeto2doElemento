using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly DetalleFacturaService _detalleService;

        public DetalleFacturaController(DetalleFacturaService detalleService)
        {
            _detalleService = detalleService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var detalles = _detalleService.GetAll();
            return Ok(detalles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detalle = await _detalleService.GetByIdAsync(id);
            if (detalle == null)
                return NotFound();

            return Ok(detalle);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DetalleFactura detalle)
        {
            await _detalleService.AddAsync(detalle);
            return Ok("Detalle agregado correctamente.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, DetalleFactura detalle)
        {
            if (id != detalle.Id)
                return BadRequest("El ID del detalle no coincide.");

            _detalleService.Update(detalle);
            return Ok("Detalle actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _detalleService.DeleteAsync(id);
            return Ok("Detalle eliminado correctamente.");
        }



        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] DetalleFacturaQueryFilter filters)
        {
            var detalles = await _detalleService.GetFilteredAsync(filters);
            return Ok(detalles);
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _detalleService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen de detalles obtenido correctamente.", data = resumen });
        }


    }
}
