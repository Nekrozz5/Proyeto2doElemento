using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutorController : ControllerBase
    {
        private readonly AutorService _autorService;

        public AutorController(AutorService autorService)
        {
            _autorService = autorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var autores = _autorService.GetAll();
            return Ok(autores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var autor = await _autorService.GetByIdAsync(id);
            if (autor == null)
                return NotFound();

            return Ok(autor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Autor autor)
        {
            await _autorService.AddAsync(autor);
            return Ok("Autor agregado correctamente.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Autor autor)
        {
            if (id != autor.Id)
                return BadRequest("El ID del autor no coincide.");

            _autorService.Update(autor);
            return Ok("Autor actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _autorService.DeleteAsync(id);
            return Ok("Autor eliminado correctamente.");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] AutorQueryFilter filters)
        {
            var autores = await _autorService.GetFilteredAsync(filters);
            return Ok(autores);
        }


    }
}
