using System;
using System.Collections.Generic;
using Libreria.Infrastructure.DTOs.DetalleFactura;

namespace Libreria.Infrastructure.DTOs.Factura
{
    public class FacturaDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        // Mapeado desde Cliente.Nombre + Apellido
        public string ClienteNombre { get; set; } = string.Empty;

        // Lista de detalles con info del libro
        public IEnumerable<DetalleFacturaDTO> Detalles { get; set; } = new List<DetalleFacturaDTO>();
    }
}
