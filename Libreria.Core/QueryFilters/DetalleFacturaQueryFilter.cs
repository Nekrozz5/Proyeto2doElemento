using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.QueryFilters
{
    public class DetalleFacturaQueryFilter : PaginationQueryFilter
    {
        public int? FacturaId { get; set; }
        public int? LibroId { get; set; }
        public string? LibroTituloContains { get; set; }
        public int? MinCantidad { get; set; }
        public int? MaxCantidad { get; set; }
        public decimal? MinPrecioUnitario { get; set; }
        public decimal? MaxPrecioUnitario { get; set; }

   
    }
}

