using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Representa el detalle de una factura, incluyendo el libro y el subtotal.
    /// </summary>
    public class DetalleFactura : BaseEntity
    {
        /// <summary>
        /// Cantidad de unidades compradas del libro.
        /// </summary>
        /// <example>2</example>
        public int Cantidad { get; set; }

        /// <summary>
        /// Precio unitario del libro.
        /// </summary>
        /// <example>85.50</example>
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Subtotal correspondiente al detalle (cantidad * precio unitario).
        /// </summary>
        /// <example>171.00</example>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Identificador de la factura asociada.
        /// </summary>
        /// <example>10</example>
        public int FacturaId { get; set; }

        /// <summary>
        /// Factura relacionada con el detalle.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Factura a la que pertenece el detalle.")]
        public virtual Factura? Factura { get; set; }

        /// <summary>
        /// Identificador del libro vendido.
        /// </summary>
        /// <example>4</example>
        public int LibroId { get; set; }

        /// <summary>
        /// Libro vendido en este detalle.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Información del libro vendido en el detalle.")]
        public virtual Libro? Libro { get; set; }
    }
}
