using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Entities
{
    
    
        public partial class Factura
        {
            public int Id { get; set; }
            public int ClienteId { get; set; }
            public DateTime Fecha { get; set; }
            public decimal Total { get; set; }

            public virtual Cliente Cliente { get; set; } = null!;
            public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
        }


    
}
