using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task AddAsync(Factura factura)
        {
            var cliente = await _clienteRepository.GetByIdAsync(factura.ClienteId);
            if (cliente == null)
                throw new Exception("El cliente no existe.");

            // Calcula total de factura
            double total = 0;
            foreach (var detalle in factura.DetalleFacturas)
            {
                var libro = await _libroRepository.GetByIdAsync(detalle.LibroId);
                if (libro == null)
                    throw new Exception($"El libro con ID {detalle.LibroId} no existe.");

                total += (double)(detalle.PrecioUnitario * detalle.Cantidad);

                // Opcional: reducir stock
                libro.Stock -= detalle.Cantidad;
                await _libroRepository.UpdateAsync(libro);
            }

            factura.Total = total;
            factura.Fecha = DateTime.Now;

            await _facturaRepository.AddAsync(factura);
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
