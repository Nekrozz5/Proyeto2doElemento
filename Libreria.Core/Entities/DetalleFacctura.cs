using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Entities
{
    public class DetalleFactura
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;

        
        public int FacturaId { get; set; }
        public virtual Factura Factura { get; set; }

       
        public int LibroId { get; set; }
        public virtual Libro Libro { get; set; }
    }
}
