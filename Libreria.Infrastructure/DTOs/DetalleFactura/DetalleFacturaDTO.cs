namespace Libreria.Infrastructure.DTOs.DetalleFactura
{
    public class DetalleFacturaDTO
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        // Información del libro asociada (por AutoMapper)
        public string LibroTitulo { get; set; } = string.Empty;
        public decimal LibroPrecio { get; set; }
    }
}
