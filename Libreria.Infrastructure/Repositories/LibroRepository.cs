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
    public class LibroRepository : ILibroRepository
    {
        private readonly ApplicationDbContext _context;

        public LibroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Libro>> GetAllAsync() =>
            await _context.Libros.ToListAsync();

        public async Task<Libro?> GetByIdAsync(int id) =>
            await _context.Libros.FindAsync(id);

        public async Task AddAsync(Libro libro)
        {
            await _context.Libros.AddAsync(libro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Libro libro)
        {
            _context.Libros.Update(libro);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Libro libro)
        {
            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Libro>> GetLibrosPorAutorAsync(int autorId) =>
            await _context.Libros.Where(l => l.AutorId == autorId).ToListAsync();
    }
}