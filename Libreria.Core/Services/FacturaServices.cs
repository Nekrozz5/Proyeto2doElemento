using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class FacturaService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILibroRepository _libroRepository;

        public FacturaService(
            IFacturaRepository facturaRepository,
            IClienteRepository clienteRepository,
            ILibroRepository libroRepository)
        {
            _facturaRepository = facturaRepository;
            _clienteRepository = clienteRepository;
            _libroRepository = libroRepository;
        }

        public async Task<IEnumerable<Factura>> GetAllAsync()
        {
            return await _facturaRepository.GetAllAsync();
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _facturaRepository.GetByIdAsync(id);
        }
        public Task<IEnumerable<Factura>> GetAllWithDetailsAsync()
    => _facturaRepository.GetAllWithDetailsAsync();

        public Task<Factura?> GetByIdWithDetailsAsync(int id)
            => _facturaRepository.GetByIdWithDetailsAsync(id);
        public async Task AddAsync(Factura factura)
        {
            factura.Fecha = DateTime.Now;
            decimal total = 0;

            foreach (var df in factura.DetalleFacturas)
            {
                var libro = await _libroRepository.GetByIdAsync(df.LibroId);
                if (libro == null) throw new Exception($"Libro {df.LibroId} no existe.");

                df.PrecioUnitario = libro.Precio;
                df.Subtotal = df.Cantidad * df.PrecioUnitario;

                // Actualiza stock (si tu patrón actual lo hace aquí)
                libro.Stock -= df.Cantidad;
                await _libroRepository.UpdateAsync(libro);

                total += df.Subtotal;
            }

            factura.Total = total;

            await _facturaRepository.AddAsync(factura); // tu repo ya guarda (fix anterior)
        }


        public async Task UpdateAsync(Factura factura)
        {
            await _facturaRepository.UpdateAsync(factura);
        }

        public async Task DeleteAsync(int id)
        {
            var factura = await _facturaRepository.GetByIdAsync(id);
            if (factura == null)
                throw new Exception("La factura no existe.");

            await _facturaRepository.DeleteAsync(factura);
        }
    }
}
