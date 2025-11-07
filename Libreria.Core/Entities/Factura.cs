using System.ComponentModel.DataAnnotations.Schema;

namespace Libreria.Core.Entities
{
    public class Factura : BaseEntity
    {
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; } // ← antes no anulable
        public DateTime Fecha { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }



        public ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    }
}
