using Libreria.Infrastructure.DTOs.Libro;

namespace Libreria.Infrastructure.DTOs.Autor
{
    public class AutorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public List<LibroDto> Libros { get; set; } = new();
    }
}
