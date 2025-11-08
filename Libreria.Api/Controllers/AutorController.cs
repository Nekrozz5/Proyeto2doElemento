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
        /// Controlador encargado de la gestión de autores.
        /// </summary>
        /// <remarks>
        /// Permite realizar operaciones CRUD sobre autores, aplicar filtros y generar reportes resumidos.
        /// </remarks>
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
            /// <response code="200">Autores obtenidos correctamente.</response>
            [HttpGet]
            [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Autor>))]
            public IActionResult GetAll()
            {
                var autores = _autorService.GetAll();
                return Ok(autores);
            }

            /// <summary>
            /// Obtiene un autor por su identificador.
            /// </summary>
            /// <param name="id">Identificador del autor.</param>
            /// <response code="200">Autor encontrado correctamente.</response>
            /// <response code="404">Autor no encontrado.</response>
            [HttpGet("{id}")]
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
            /// <param name="autor">Datos del autor.</param>
            /// <response code="201">Autor creado correctamente.</response>
            [HttpPost]
            [ProducesResponseType((int)HttpStatusCode.Created)]
            public async Task<IActionResult> Create(Autor autor)
            {
                await _autorService.AddAsync(autor);
                return StatusCode((int)HttpStatusCode.Created, "Autor agregado correctamente.");
            }

            /// <summary>
            /// Actualiza un autor existente.
            /// </summary>
            /// <param name="id">Identificador del autor.</param>
            /// <param name="autor">Datos actualizados.</param>
            /// <response code="200">Autor actualizado correctamente.</response>
            [HttpPut("{id}")]
            [ProducesResponseType((int)HttpStatusCode.OK)]
            public IActionResult Update(int id, Autor autor)
            {
                if (id != autor.Id)
                    return BadRequest("El ID del autor no coincide.");

                _autorService.Update(autor);
                return Ok("Autor actualizado correctamente.");
            }

            /// <summary>
            /// Elimina un autor por su identificador.
            /// </summary>
            /// <param name="id">Identificador del autor.</param>
            /// <response code="200">Autor eliminado correctamente.</response>
            [HttpDelete("{id}")]
            [ProducesResponseType((int)HttpStatusCode.OK)]
            public async Task<IActionResult> Delete(int id)
            {
                await _autorService.DeleteAsync(id);
                return Ok("Autor eliminado correctamente.");
            }

            /// <summary>
            /// Filtra autores según criterios (nombre, apellido, etc.) y devuelve resultados paginados.
            /// </summary>
            /// <param name="filters">Filtros de búsqueda.</param>
            /// <response code="200">Autores filtrados correctamente.</response>
            [HttpGet("filter")]
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
            /// Obtiene un resumen de autores y sus libros publicados.
            /// </summary>
            /// <response code="200">Resumen obtenido correctamente.</response>
            [HttpGet("resumen")]
            [ProducesResponseType((int)HttpStatusCode.OK)]
            public async Task<IActionResult> GetResumen()
            {
                var resumen = await _autorService.GetResumenAsync();
                return Ok(new { mensaje = "Resumen de autores obtenido correctamente.", data = resumen });
            }
        }
    }
