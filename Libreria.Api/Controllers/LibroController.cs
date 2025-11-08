using AutoMapper;
using Libreria.Api.Responses;
using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Libreria.Core.Enums;
using Libreria.Core.QueryFilters;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Libro;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Libreria.Api.Controllers
{
    /// <summary>
    /// Controlador encargado de administrar los recursos de Libros.
    /// </summary>
    /// <remarks>
    /// Permite realizar operaciones CRUD, aplicar filtros, y obtener listados paginados de libros.
    /// </remarks>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase
    {
        private readonly LibroService _libroService;
        private readonly IMapper _mapper;

        public LibroController(LibroService libroService, IMapper mapper)
        {
            _libroService = libroService;
            _mapper = mapper;
        }

        /// <summary>
        /// Recupera todos los libros registrados en el sistema.
        /// </summary>
        /// <response code="200">Lista de libros obtenida correctamente.</response>
        /// <response code="404">No se encontraron registros.</response>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<LibroDto>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetAll()
        {
            var libros = _libroService.GetAll();
            var librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return Ok(librosDto);
        }

        /// <summary>
        /// Recupera un libro por su identificador único.
        /// </summary>
        /// <param name="id">Identificador del libro.</param>
        /// <response code="200">Libro encontrado y retornado correctamente.</response>
        /// <response code="404">No existe un libro con el ID proporcionado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LibroDto))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null)
                return NotFound();

            var libroDto = _mapper.Map<LibroDto>(libro);
            return Ok(libroDto);
        }

        /// <summary>
        /// Crea un nuevo libro en la base de datos.
        /// </summary>
        /// <param name="dto">Objeto con los datos del libro a crear.</param>
        /// <response code="201">Libro creado correctamente.</response>
        /// <response code="400">Datos de entrada inválidos.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(LibroDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] LibroCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var libro = _mapper.Map<Libro>(dto);
            await _libroService.AddAsync(libro);

            var libroCompleto = await _libroService.GetByIdAsync(libro.Id);
            var libroDto = _mapper.Map<LibroDto>(libroCompleto);

            return StatusCode((int)HttpStatusCode.Created, libroDto);
        }

        /// <summary>
        /// Actualiza la información de un libro existente.
        /// </summary>
        /// <param name="id">Identificador del libro a modificar.</param>
        /// <param name="dto">Datos actualizados del libro.</param>
        /// <response code="200">Libro actualizado correctamente.</response>
        /// <response code="400">El ID del libro no coincide o los datos son inválidos.</response>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Update(int id, [FromBody] LibroUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID del libro no coincide.");

            var libro = _mapper.Map<Libro>(dto);
            _libroService.Update(libro);

            return Ok("Libro actualizado correctamente.");
        }

        /// <summary>
        /// Elimina un libro por su identificador.
        /// </summary>
        /// <param name="id">Identificador del libro a eliminar.</param>
        /// <response code="200">Libro eliminado correctamente.</response>
        /// <response code="404">El libro no existe.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _libroService.DeleteAsync(id);
            return Ok("Libro eliminado correctamente.");
        }

        /// <summary>
        /// Recupera una lista paginada de libros aplicando filtros de búsqueda.
        /// </summary>
        /// <param name="filters">Filtros de búsqueda y paginación.</param>
        /// <returns>Un objeto paginado con los resultados encontrados.</returns>
        /// <response code="200">Registros de libros recuperados correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("filter")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<LibroDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFiltered([FromQuery] LibroQueryFilter filters)
        {
            try
            {
                var result = await _libroService.GetFilteredAsync(filters);

                var libros = result.Pagination.Cast<Libro>().ToList();
                var librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);

                var pagination = new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                };

                var response = new ApiResponse<IEnumerable<LibroDto>>(librosDto)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (System.Exception ex)
            {
                var errorResponse = new ResponseData
                {
                    Messages = new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.error.ToString(),
                            Description = ex.Message
                        }
                    },
                    StatusCode = HttpStatusCode.InternalServerError
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
