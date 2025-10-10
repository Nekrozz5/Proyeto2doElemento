using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Factura
{
    public class FacturaUpdateDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
    }
}
