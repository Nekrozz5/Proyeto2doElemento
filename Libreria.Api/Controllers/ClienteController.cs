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
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        [HttpGet]
        [Authorize]   // <--- User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Cliente>))]
        public IActionResult GetAll()
        {
            var clientes = _clienteService.GetAll();
            return Ok(clientes);
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]   // <--- User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Cliente))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        /// <summary>
        /// Crea un cliente.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]  // <--- SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            await _clienteService.AddAsync(cliente);
            return StatusCode((int)HttpStatusCode.Created, "Cliente agregado correctamente.");
        }

        /// <summary>
        /// Actualiza un cliente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // <--- SOLO ADMIN
        public async Task<IActionResult> Put(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest("El ID no coincide.");

            await _clienteService.UpdateAsync(cliente);
            return NoContent();
        }

        /// <summary>
        /// Elimina un cliente.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // <--- SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteService.DeleteAsync(id);
            return Ok("Cliente eliminado correctamente.");
        }

        /// <summary>
        /// Filtra clientes con paginación.
        /// </summary>
        [HttpGet("filter")]
        [Authorize]   // <--- User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Cliente>>))]
        public async Task<IActionResult> GetFiltered([FromQuery] ClienteQueryFilter filters)
        {
            var clientes = await _clienteService.GetFilteredAsync(filters);

            var pagination = new Pagination
            {
                TotalCount = clientes.TotalCount,
                PageSize = clientes.PageSize,
                CurrentPage = clientes.CurrentPage,
                TotalPages = clientes.TotalPages,
                HasNextPage = clientes.HasNextPage,
                HasPreviousPage = clientes.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<Cliente>>(clientes)
            {
                Pagination = pagination
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un resumen de clientes y facturas.
        /// </summary>
        [HttpGet("resumen")]
        [Authorize]  // <--- User o Admin
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _clienteService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen de clientes obtenido correctamente.", data = resumen });
        }
    }
}
