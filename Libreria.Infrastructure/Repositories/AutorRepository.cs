using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Infrastructure.Repositories
{
    public class AutorRepository : IAutorRepository
    {
        private readonly ApplicationDbContext _context;

        public AutorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Autor>> GetAllAsync()
        {
            return await _context.Autores
                .Include(a => a.Libros) 
                .ToListAsync();
        }
        public async Task<Autor?> GetByIdAsync(int id)
        {
            return await _context.Autores
                .Include(a => a.Libros)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task AddAsync(Autor autor)
        {
            await _context.Autores.AddAsync(autor);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Autor autor)
        {
            _context.Autores.Update(autor);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Autor autor)
        {
            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
        }
    }
}

