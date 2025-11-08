using System.ComponentModel.DataAnnotations.Schema;

using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Representa una factura emitida a un cliente.
    /// </summary>
    /// <remarks>
    /// Contiene la información del cliente, fecha de emisión, total y los detalles asociados.
    /// </remarks>
    public class Factura : BaseEntity
    {
        /// <summary>
        /// Identificador del cliente que realizó la compra.
        /// </summary>
        /// <example>5</example>
        public int ClienteId { get; set; }

        /// <summary>
        /// Información del cliente asociado a la factura.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Cliente al que pertenece la factura.")]
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Fecha en que se emitió la factura.
        /// </summary>
        /// <example>2025-11-07</example>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Total de la factura expresado en bolivianos.
        /// </summary>
        /// <example>250.75</example>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        /// <summary>
        /// Lista de detalles incluidos en la factura.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Colección de detalles que conforman la factura.")]
        public ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
    }
}
