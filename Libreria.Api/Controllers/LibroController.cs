using Libreria.Core.Entities;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibroController : ControllerBase
    {
        private readonly LibroService _libroService;

        public LibroController(LibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var libros = _libroService.GetAll();
            return Ok(libros);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null)
                return NotFound();

            return Ok(libro);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Libro libro)
        {
            await _libroService.AddAsync(libro);
            return Ok("Libro agregado correctamente.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Libro libro)
        {
            if (id != libro.Id)
                return BadRequest("El ID del libro no coincide.");

            _libroService.Update(libro);
            return Ok("Libro actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _libroService.DeleteAsync(id);
            return Ok("Libro eliminado correctamente.");
        }
    }
}
