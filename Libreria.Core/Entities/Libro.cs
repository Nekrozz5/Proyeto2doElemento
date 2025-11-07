namespace Libreria.Core.Entities
{
    public  class Libro : BaseEntity
    {
        public string Titulo { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Relación con Autor
        public int AutorId { get; set; }
        public virtual Autor? Autor { get; set; }
    }

}
