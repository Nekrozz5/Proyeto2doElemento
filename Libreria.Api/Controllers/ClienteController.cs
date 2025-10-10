using AutoMapper;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.DTOs.Cliente;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _repo;
        private readonly IMapper _mapper;

        public ClienteController(IClienteRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            var clientes = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ClienteCreateDto dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);
            await _repo.AddAsync(cliente);
            return CreatedAtAction(nameof(GetAll), new { id = cliente.Id }, _mapper.Map<ClienteDto>(cliente));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ClienteUpdateDto dto)
        {
            var cliente = await _repo.GetByIdAsync(id);
            if (cliente == null) return NotFound();

            _mapper.Map(dto, cliente);
            await _repo.UpdateAsync(cliente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var cliente = await _repo.GetByIdAsync(id);
            if (cliente == null) return NotFound();

            await _repo.DeleteAsync(cliente);
            return NoContent();
        }
    }
}
