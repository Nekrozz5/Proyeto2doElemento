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
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController : ControllerBase
    {
        private readonly LibroService _libroService;
        private readonly IMapper _mapper;

        public LibroController(LibroService libroService, IMapper mapper)
        {
            _libroService = libroService;
            _mapper = mapper;
        }

        // ========================================
        // CRUD BÁSICO (sin cambios)
        // ========================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var libros = _libroService.GetAll();
            var librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return Ok(librosDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null)
                return NotFound();

            var libroDto = _mapper.Map<LibroDto>(libro);
            return Ok(libroDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LibroCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var libro = _mapper.Map<Libro>(dto);
            await _libroService.AddAsync(libro);

            var libroCompleto = await _libroService.GetByIdAsync(libro.Id);
            var libroDto = _mapper.Map<LibroDto>(libroCompleto);

            return Ok(libroDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LibroUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID del libro no coincide.");

            var libro = _mapper.Map<Libro>(dto);
            _libroService.Update(libro);

            return Ok("Libro actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _libroService.DeleteAsync(id);
            return Ok("Libro eliminado correctamente.");
        }

        // ========================================
        // FILTRO CON PAGINACIÓN + MESSAGES
        // ========================================
        [HttpGet("filter")]
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
