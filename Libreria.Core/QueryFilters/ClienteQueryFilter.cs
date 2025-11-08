using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.QueryFilters
{
    public class ClienteQueryFilter
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? EmailContains { get; set; }
        public bool? ConFacturas { get; set; }
    }
}

