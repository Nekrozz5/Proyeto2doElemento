using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.DTOs.Factura;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaRepository _repo;
        private readonly IMapper _mapper;

        public FacturaController(IFacturaRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDTO>>> GetAll()
        {
            var facturas = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<FacturaDTO>>(facturas));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDTO>> GetById(int id)
        {
            var factura = await _repo.GetByIdAsync(id);
            if (factura == null) return NotFound();

            return Ok(_mapper.Map<FacturaDTO>(factura));
        }

        [HttpPost]
        public async Task<ActionResult> Create(FacturaCreateDTO dto)
        {
            var factura = _mapper.Map<Factura>(dto);
            factura.Fecha = DateTime.Now;

            await _repo.AddAsync(factura);
            return CreatedAtAction(nameof(GetById), new { id = factura.Id }, _mapper.Map<FacturaDTO>(factura));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var factura = await _repo.GetByIdAsync(id);
            if (factura == null) return NotFound();

            await _repo.DeleteAsync(factura);
            return NoContent();
        }
    }
}
