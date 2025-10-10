using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Factura
{
    public class FacturaCreateDTO
    {
        public int ClienteId { get; set; }
        public List<FacturaDetalleDTO> Detalles { get; set; } = new();
    }
}
