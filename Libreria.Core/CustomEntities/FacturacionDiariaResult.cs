using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.CustomEntities
{
    public class FacturacionDiariaResult
    {
        public DateTime Fecha { get; set; }
        public string Dia { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public int CantidadLibros { get; set; }
    }
}
