using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libreria.Infrastructure.DTOs.DetalleFactura;


namespace Libreria.Infrastructure.DTOs.Factura
{
    public class FacturaDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public string ClienteNombre { get; set; } = string.Empty;

        public List<DetalleFacturaDTO> Detalles { get; set; } = new();
    }
}
