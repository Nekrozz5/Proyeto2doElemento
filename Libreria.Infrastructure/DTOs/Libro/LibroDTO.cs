using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Libro
{
    public class LibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int? AnioPublicacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int AutorId { get; set; }

        // ✅ Solo mostramos el nombre completo del autor
        public string AutorNombre { get; set; } = string.Empty;
    }

}
