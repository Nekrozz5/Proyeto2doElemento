using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.DTOs.Libro;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController : ControllerBase
    {
        private readonly ILibroRepository _repo;
        private readonly IMapper _mapper;

        public LibroController(ILibroRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // ✅ GET: api/libro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibroDto>>> GetAll()
        {
            var libros = await _repo.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<LibroDto>>(libros);
            return Ok(dto);
        }

        // ✅ GET: api/libro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LibroDto>> GetById(int id)
        {
            var libro = await _repo.GetByIdAsync(id);
            if (libro == null) return NotFound();

            var dto = _mapper.Map<LibroDto>(libro);
            return Ok(dto);
        }

        // ✅ POST: api/libro
        [HttpPost]
        public async Task<ActionResult> Create(LibroCreateDto dto)
        {
            var libro = _mapper.Map<Libro>(dto);
            await _repo.AddAsync(libro);
            return CreatedAtAction(nameof(GetById), new { id = libro.Id }, _mapper.Map<LibroDto>(libro));
        }

        // ✅ PUT: api/libro/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, LibroUpdateDto dto)
        {
            var libro = await _repo.GetByIdAsync(id);
            if (libro == null) return NotFound();

            _mapper.Map(dto, libro);
            await _repo.UpdateAsync(libro);
            return NoContent();
        }

        // ✅ DELETE: api/libro/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var libro = await _repo.GetByIdAsync(id);
            if (libro == null) return NotFound();

            await _repo.DeleteAsync(libro);
            return NoContent();
        }
    }
}
