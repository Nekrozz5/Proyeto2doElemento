using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Api.Responses;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libreria.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de detalles de facturas.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly DetalleFacturaService _detalleService;

        public DetalleFacturaController(DetalleFacturaService detalleService)
        {
            _detalleService = detalleService;
        }

        /// <summary>
        /// Obtiene todos los detalles registrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<DetalleFactura>))]
        public IActionResult GetAll()
        {
            var detalles = _detalleService.GetAll();
            return Ok(detalles);
        }

        /// <summary>
        /// Obtiene un detalle por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DetalleFactura))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var detalle = await _detalleService.GetByIdAsync(id);
            if (detalle == null)
                return NotFound();

            return Ok(detalle);
        }

        /// <summary>
        /// Crea un nuevo detalle de factura.
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(DetalleFactura detalle)
        {
            await _detalleService.AddAsync(detalle);
            return StatusCode((int)HttpStatusCode.Created, "Detalle agregado correctamente.");
        }

        /// <summary>
        /// Actualiza un detalle existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, DetalleFactura detalle)
        {
            if (id != detalle.Id)
                return BadRequest("El ID no coincide.");

            await _detalleService.UpdateAsync(detalle); // ← ahora sí existe
            return NoContent();
        }



        /// <summary>
        /// Elimina un detalle por ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _detalleService.DeleteAsync(id);
            return Ok("Detalle eliminado correctamente.");
        }

        /// <summary>
        /// Filtra detalles de facturas según criterios de búsqueda.
        /// </summary>
        [HttpGet("filter")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<DetalleFactura>>))]
        public async Task<IActionResult> GetFiltered([FromQuery] DetalleFacturaQueryFilter filters)
        {
            var detalles = await _detalleService.GetFilteredAsync(filters);

            var pagination = new Pagination
            {
                TotalCount = detalles.TotalCount,
                PageSize = detalles.PageSize,
                CurrentPage = detalles.CurrentPage,
                TotalPages = detalles.TotalPages,
                HasNextPage = detalles.HasNextPage,
                HasPreviousPage = detalles.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<DetalleFactura>>(detalles)
            {
                Pagination = pagination
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un resumen de todos los detalles con información de libro.
        /// </summary>
        [HttpGet("resumen")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _detalleService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen de detalles obtenido correctamente.", data = resumen });
        }
    }
}
