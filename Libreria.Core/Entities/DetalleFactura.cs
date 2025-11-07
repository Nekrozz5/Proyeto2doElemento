using System.ComponentModel.DataAnnotations.Schema;

namespace Libreria.Core.Entities
{
    public class DetalleFactura : BaseEntity
    {
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        public int FacturaId { get; set; }
        public virtual Factura Factura { get; set; } = null!;

        public int LibroId { get; set; }
        public virtual Libro Libro { get; set; } = null!;
    }
}
