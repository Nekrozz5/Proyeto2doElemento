using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Api.Responses;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Libreria.Api.Controllers
{
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
        /// Obtiene todos los detalles.
        /// </summary>
        [HttpGet]
        [Authorize] // USER o ADMIN
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<DetalleFactura>))]
        public IActionResult GetAll()
        {
            var detalles = _detalleService.GetAll();
            return Ok(detalles);
        }

        /// <summary>
        /// Obtiene un detalle por ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize] // USER o ADMIN
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DetalleFactura))]
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
        [Authorize(Roles = "Admin")] // SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(DetalleFactura detalle)
        {
            await _detalleService.AddAsync(detalle);
            return StatusCode((int)HttpStatusCode.Created, "Detalle agregado correctamente.");
        }

        /// <summary>
        /// Actualiza un detalle.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // SOLO ADMIN
        public async Task<IActionResult> Put(int id, DetalleFactura detalle)
        {
            if (id != detalle.Id)
                return BadRequest("El ID no coincide.");

            await _detalleService.UpdateAsync(detalle);
            return NoContent();
        }

        /// <summary>
        /// Elimina un detalle.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _detalleService.DeleteAsync(id);
            return Ok("Detalle eliminado correctamente.");
        }

        /// <summary>
        /// Filtra detalles con paginación.
        /// </summary>
        [HttpGet("filter")]
        [Authorize] // USER o ADMIN
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
        /// Obtiene un resumen con información del libro.
        /// </summary>
        [HttpGet("resumen")]
        [Authorize] // USER o ADMIN
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _detalleService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen de detalles obtenido correctamente.", data = resumen });
        }
    }
}
