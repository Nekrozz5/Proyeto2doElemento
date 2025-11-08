using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Representa a un cliente dentro del sistema de Librería.
    /// </summary>
    /// <remarks>
    /// Contiene los datos personales del cliente y su historial de facturas.
    /// </remarks>
    public class Cliente : BaseEntity
    {
        /// <summary>
        /// Nombre del cliente.
        /// </summary>
        /// <example>Ariel</example>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del cliente.
        /// </summary>
        /// <example>Zeballos</example>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del cliente.
        /// </summary>
        /// <example>ariel.zeballos@ucb.edu.bo</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Facturas asociadas al cliente.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Listado de facturas realizadas por el cliente.")]
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}
