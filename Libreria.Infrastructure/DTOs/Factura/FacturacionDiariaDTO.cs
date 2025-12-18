using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Factura
{
    public class FacturacionDiariaDTO
    {
        public DateTime Fecha { get; set; }
        public string Dia { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public int CantidadLibros { get; set; }
    }
}
