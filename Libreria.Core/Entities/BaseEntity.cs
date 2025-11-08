using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Clase base que contiene propiedades comunes para todas las entidades.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador único del registro.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de creación del registro (UTC).
        /// </summary>
        /// <example>2025-11-07T20:00:00Z</example>
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de la última actualización del registro (UTC).
        /// </summary>
        /// <example>2025-11-07T21:30:00Z</example>
        public DateTime? FechaActualizacion { get; set; }
    }
}
