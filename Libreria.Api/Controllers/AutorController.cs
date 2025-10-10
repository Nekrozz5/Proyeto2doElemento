using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.DTOs.Autor;
using Libreria.Infrastructure.DTOs.Autor.Libreria.Api.DTOs.Autor;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutorController : ControllerBase
    {
        private readonly IAutorRepository _repo;
        private readonly IMapper _mapper;

        public AutorController(IAutorRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDTO>>> GetAll()
        {
            var autores = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<AutorDTO>>(autores));
        }

        [HttpPost]
        public async Task<ActionResult> Create(AutorCreateDto dto)
        {
            var autor = _mapper.Map<Autor>(dto);
            await _repo.AddAsync(autor);
            return CreatedAtAction(nameof(GetAll), new { id = autor.Id }, _mapper.Map<AutorDTO>(autor));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, AutorUpdateDto dto)
        {
            var autor = await _repo.GetByIdAsync(id);
            if (autor == null) return NotFound();

            _mapper.Map(dto, autor);
            await _repo.UpdateAsync(autor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor = await _repo.GetByIdAsync(id);
            if (autor == null) return NotFound();

            await _repo.DeleteAsync(autor);
            return NoContent();
        }
    }
}
