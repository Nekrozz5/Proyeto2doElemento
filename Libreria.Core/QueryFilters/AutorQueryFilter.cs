using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.QueryFilters
{
    public class AutorQueryFilter
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public bool? ConLibros { get; set; }  // true => solo autores con >=1 libro
        
    }
}
