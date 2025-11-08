using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Representa un libro dentro del sistema de Librería.
    /// </summary>
    /// <remarks>
    /// Contiene los datos principales de los libros como título, autor, precio y stock disponible.
    /// </remarks>
    public class Libro : BaseEntity
    {
        /// <summary>
        /// Título del libro.
        /// </summary>
        /// <example>El Hobbit</example>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Año de publicación del libro.
        /// </summary>
        /// <example>1937</example>
        public int? AnioPublicacion { get; set; }

        /// <summary>
        /// Breve descripción o sinopsis del libro.
        /// </summary>
        /// <example>Una aventura épica de un hobbit en busca de un tesoro custodiado por un dragón.</example>
        public string Descripcion { get; set; } = string.Empty;

        /// <summary>
        /// Precio actual del libro.
        /// </summary>
        /// <example>85.50</example>
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Cantidad disponible en inventario.
        /// </summary>
        /// <example>10</example>
        public int Stock { get; set; }

        /// <summary>
        /// Identificador del autor asociado.
        /// </summary>
        /// <example>3</example>
        public int AutorId { get; set; }

        /// <summary>
        /// Información del autor relacionada al libro.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Información del autor relacionada al libro.")]
        public virtual Autor? Autor { get; set; }
    }
}
