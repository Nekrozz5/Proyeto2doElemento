using AutoMapper;
using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Api.Responses;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Factura;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libreria.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de facturas.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _facturaService;
        private readonly IMapper _mapper;

        public FacturaController(FacturaService facturaService, IMapper mapper)
        {
            _facturaService = facturaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las facturas con detalles y cliente.
        /// </summary>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FacturaDTO>))]
        public async Task<IActionResult> GetAll()
        {
            var facturas = await _facturaService.GetAllAsync(); // ✔ usa GetAllAsync
            var facturasDto = _mapper.Map<IEnumerable<FacturaDTO>>(facturas);
            return Ok(facturasDto);
        }

        /// <summary>
        /// Obtiene una factura por ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FacturaDTO))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            if (factura == null)
                return NotFound();

            var facturaDto = _mapper.Map<FacturaDTO>(factura);
            return Ok(facturaDto);
        }

        /// <summary>
        /// Crea una nueva factura.
        /// </summary>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] FacturaCreateDTO dto)
        {
            var factura = _mapper.Map<Factura>(dto);
            await _facturaService.AddAsync(factura);

            var facturaCompleta = await _facturaService.GetByIdAsync(factura.Id);
            var facturaDto = _mapper.Map<FacturaDTO>(facturaCompleta);

            return StatusCode((int)HttpStatusCode.Created, facturaDto);
        }

        /// <summary>
        /// Actualiza una factura existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] Factura factura)
        {
            if (id != factura.Id)
                return BadRequest("El ID de la factura no coincide.");

            await _facturaService.UpdateAsync(factura); // ✔ usa UpdateAsync
            return Ok("Factura actualizada correctamente.");
        }

        /// <summary>
        /// Elimina una factura.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _facturaService.DeleteAsync(id);
            return Ok("Factura eliminada correctamente.");
        }

        /// <summary>
        /// Filtra facturas por cliente, fecha o monto total.
        /// </summary>
        [HttpGet("filter")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Factura>>))]
        public async Task<IActionResult> GetFiltered([FromQuery] FacturaQueryFilter filters)
        {
            var facturas = await _facturaService.GetFilteredAsync(filters);

            var pagination = new Pagination
            {
                TotalCount = facturas.TotalCount,
                PageSize = facturas.PageSize,
                CurrentPage = facturas.CurrentPage,
                TotalPages = facturas.TotalPages,
                HasNextPage = facturas.HasNextPage,
                HasPreviousPage = facturas.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<Factura>>(facturas)
            {
                Pagination = pagination
            };

            return Ok(response);
        }
    }
}
