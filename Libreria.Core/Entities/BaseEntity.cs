using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
    }
}
