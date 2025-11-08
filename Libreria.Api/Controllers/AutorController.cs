using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Api.Responses;
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

        // ==================================================
        // GET: api/autor/resumen (DAPPER)
        // ==================================================
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = await _autorService.GetResumenAsync();
            return Ok(new { mensaje = "Resumen de autores obtenido correctamente.", data = resumen });
        }

    }
}
