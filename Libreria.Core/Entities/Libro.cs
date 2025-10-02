namespace Libreria.Core.Entities
{
    public partial class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public double Precio { get; set; }
        public int Stock { get; set; }

        // Relación con Autor
        public int AutorId { get; set; }
        public virtual Autor Autor { get; set; }
    }

}
