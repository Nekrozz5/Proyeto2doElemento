using Libreria.Core.Entities;
using Libreria.Core.QueryFilters;
using Libreria.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var clientes = _clienteService.GetAll();
            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            await _clienteService.AddAsync(cliente);
            return Ok("Cliente agregado correctamente.");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Cliente cliente)
        {
            if (id != cliente.Id)
                return BadRequest("El ID del cliente no coincide.");

            _clienteService.Update(cliente);
            return Ok("Cliente actualizado correctamente.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteService.DeleteAsync(id);
            return Ok("Cliente eliminado correctamente.");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] ClienteQueryFilter filters)
        {
            var clientes = await _clienteService.GetFilteredAsync(filters);
            return Ok(clientes);
        }

    }
}
