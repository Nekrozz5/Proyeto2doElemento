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
    public class AutorController : ControllerBase
    {
        private readonly AutorService _autorService;

        public AutorController(AutorService autorService)
        {
            _autorService = autorService;
        }

        /// <summary>
        /// Obtiene todos los autores registrados.
        /// </summary>
        [HttpGet]
        [Authorize]   // <--- Acceso para User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Autor>))]
        public IActionResult GetAll()
        {
            var autores = _autorService.GetAll();
            return Ok(autores);
        }

        /// <summary>
        /// Obtiene un autor por ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]   // <--- Acceso para User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Autor))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var autor = await _autorService.GetByIdAsync(id);
            if (autor == null)
                return NotFound();

            return Ok(autor);
        }

        /// <summary>
        /// Crea un nuevo autor.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]   // <--- SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(Autor autor)
        {
            await _autorService.AddAsync(autor);
            return StatusCode((int)HttpStatusCode.Created, "Autor agregado correctamente.");
        }

        /// <summary>
        /// Actualiza un autor existente.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]   // <--- SOLO ADMIN
        public async Task<IActionResult> Put(int id, Autor autor)
        {
            if (id != autor.Id)
                return BadRequest("El ID no coincide.");

            await _autorService.UpdateAsync(autor);
            return NoContent();
        }

        /// <summary>
        /// Elimina un autor.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]   // <--- SOLO ADMIN
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _autorService.DeleteAsync(id);
            return Ok("Autor eliminado correctamente.");
        }

        /// <summary>
        /// Filtrado de autores con paginación.
        /// </summary>
        [HttpGet("filter")]
        [Authorize]  // <--- Acceso para User o Admin
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Autor>>))]
        public async Task<IActionResult> GetFiltered([FromQuery] AutorQueryFilter filters)
        {
            var autores = await _autorService.GetFilteredAsync(filters);

            var pagination = new Pagination
            {
                TotalCount = autores.TotalCount,
                PageSize = autores.PageSize,
                CurrentPage = autores.CurrentPage,
                TotalPages = autores.TotalPages,
                HasNextPage = autores.HasNextPage,
                HasPreviousPage = autores.HasPreviousPage
            };

            var response = new ApiResponse<IEnumerable<Autor>>(autores)
            {
                Pagination = pagination
            };

            return Ok(response);
        }

        /// <summary>
        /// Obtiene un resumen de autores.
        /// </summary>
        [HttpGet("resumen")]
        [Authorize]  // <--- Acceso para User o Admin
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _autorService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen obtenido correctamente.", data = resumen });
        }
    }
}
