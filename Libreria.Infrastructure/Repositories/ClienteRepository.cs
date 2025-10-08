using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Libreria.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Infrastructure.Repositories
{
    public class ClienteRepository: IClienteRepository
    {
            private readonly ApplicationDbContext _context;
            public ClienteRepository(ApplicationDbContext context)
            {
                _context = context;
        }
            public async Task<IEnumerable<Cliente>> GetAllAsync() =>
                await _context.Clientes.ToListAsync();
            public async Task<Cliente?> GetByIdAsync(int id) =>
                await _context.Clientes.FindAsync(id);
            public async Task AddAsync(Cliente cliente)
            {
                await _context.Clientes.AddAsync(cliente);
                await _context.SaveChangesAsync();
            }
            public async Task UpdateAsync(Cliente cliente)
            {
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            public async Task DeleteAsync(Cliente cliente)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
        }





    }
}
