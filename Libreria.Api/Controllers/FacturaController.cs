using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Libreria.Core.Services;
using Libreria.Core.Entities;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _facturaService;

        public FacturaController(FacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        // GET: api/Factura
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var facturas = await _facturaService.GetAllAsync(); // <-- usar Async
            return Ok(facturas);
        }

        // GET: api/Factura/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var factura = await _facturaService.GetByIdAsync(id);
            if (factura is null) return NotFound();
            return Ok(factura);
        }

        // POST: api/Factura
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Factura factura)
        {
            await _facturaService.AddAsync(factura);
            return CreatedAtAction(nameof(GetById), new { id = factura.Id }, factura);
        }

        // PUT: api/Factura/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Factura factura)
        {
            if (id != factura.Id) return BadRequest("El id de la URL no coincide con el del cuerpo.");
            await _facturaService.UpdateAsync(factura);
            return NoContent();
        }

        // DELETE: api/Factura/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _facturaService.DeleteAsync(id);
            return NoContent();
        }
    }
}
