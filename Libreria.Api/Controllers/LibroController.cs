using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Core.Responses;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Libro;
using Microsoft.AspNetCore.Mvc;

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
        // GET: api/libro
        // ========================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var libros = _libroService.GetAll();
            var librosDto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return Ok(librosDto);
        }

        // ========================================
        // GET: api/libro/{id}
        // ========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null)
                return NotFound();

            var libroDto = _mapper.Map<LibroDto>(libro);
            return Ok(libroDto);
        }

        // ========================================
        // POST: api/libro
        // ========================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LibroCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var libro = _mapper.Map<Libro>(dto);
            await _libroService.AddAsync(libro);

            // Obtener libro con Autor cargado
            var libroCompleto = await _libroService.GetByIdAsync(libro.Id);
            var libroDto = _mapper.Map<LibroDto>(libroCompleto);

            return Ok(libroDto);
        }

        // ========================================
        // PUT: api/libro/{id}
        // ========================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LibroUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID del libro no coincide.");

            var libro = _mapper.Map<Libro>(dto);
            _libroService.Update(libro);

            return Ok("Libro actualizado correctamente.");
        }

        // ========================================
        // DELETE: api/libro/{id}
        // ========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _libroService.DeleteAsync(id);
            return Ok("Libro eliminado correctamente.");
        }


        // libro filter


        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] LibroQueryFilter filters)
        {
            var libros = await _libroService.GetFilteredAsync(filters);
            return Ok(new ApiResponse<object>(libros, "Libros filtrados correctamente."));
        }
    }
}
