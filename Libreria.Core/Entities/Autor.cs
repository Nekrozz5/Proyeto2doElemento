using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swashbuckle.AspNetCore.Annotations;

namespace Libreria.Core.Entities
{
    /// <summary>
    /// Representa un autor dentro del sistema de Librería.
    /// </summary>
    /// <remarks>
    /// Contiene los datos personales del autor y su relación con los libros publicados.
    /// </remarks>
    public class Autor : BaseEntity
    {
        /// <summary>
        /// Nombre del autor.
        /// </summary>
        /// <example>J.R.R.</example>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del autor.
        /// </summary>
        /// <example>Tolkien</example>
        public string Apellido { get; set; } = string.Empty;

        /// <summary>
        /// Lista de libros escritos por el autor.
        /// </summary>
        [SwaggerSchema(ReadOnly = true, Description = "Colección de libros asociados al autor.")]
        public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}

