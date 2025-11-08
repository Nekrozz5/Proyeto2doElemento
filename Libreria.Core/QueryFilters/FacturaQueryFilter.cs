using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Libreria.Core.QueryFilters
{
    public class FacturaQueryFilter : PaginationQueryFilter
    {
        public int? ClienteId { get; set; }
        public string? ClienteNombreContains { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }

       
    }
}
