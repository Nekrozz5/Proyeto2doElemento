using Libreria.Infrastructure.DTOs.Libro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Autor
{
    namespace Libreria.Api.DTOs.Autor
    {
        public class AutorDTO
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public List<LibroDto> Libros { get; set; } = new();

        }
    }

}
