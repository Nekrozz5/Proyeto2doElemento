using Libreria.Core.Entities;
using AutoMapper;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Factura;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly FacturaService _facturaService;
        private readonly IMapper _mapper;
        public FacturaController(FacturaService facturaService, IMapper mapper) // ✅ recibe IMapper
        {
            _facturaService = facturaService;
            _mapper = mapper; // ✅ asigna aquí
        }



        // GET: api/Factura
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetAll()
        {
            var facturas = await _facturaService.GetAllWithDetailsAsync();
            var dto = _mapper.Map<IEnumerable<FacturaDTO>>(facturas);
            return Ok(dto);
        }
        // GET: api/Factura/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FacturaDTO>> GetById(int id)
        {
            var factura = await _facturaService.GetByIdWithDetailsAsync(id);
            if (factura == null) return NotFound();

            var dto = _mapper.Map<FacturaDTO>(factura);
            return Ok(dto);
        }

       
       

        // POST: api/Factura
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FacturaCreateDTO dto)
        {
            var factura = _mapper.Map<Factura>(dto);
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
