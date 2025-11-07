using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.Libro
{
    public class LibroUpdateDto
    {
        public int Id { get; set; }                  // 🔹 Necesario para identificar el libro
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int? AnioPublicacion { get; set; }    // 🔹 Opcional (puede venir null)
        public decimal Precio { get; set; }          // 🔹 Usa decimal para coincidir con la entidad
        public int Stock { get; set; }
        public int AutorId { get; set; }
    }

}
