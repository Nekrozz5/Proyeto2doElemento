namespace Libreria.Core.Entities
{
    public class Factura : BaseEntity
    {
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }

        public ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
    }
}
