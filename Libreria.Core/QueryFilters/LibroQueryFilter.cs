namespace Libreria.Core.QueryFilters
{
    public class LibroQueryFilter
    {
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public decimal? MinPrecio { get; set; }
        public decimal? MaxPrecio { get; set; }
        public bool? Disponibles { get; set; }
    }
}
