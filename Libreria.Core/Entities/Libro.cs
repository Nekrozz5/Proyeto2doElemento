using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Entities
{
    public partial class Libro
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }

        public int AnioPublicacion { get; set; }

        public string Descripcion { get; set; }

        public double Precio { get; set; }
        public int Stock { get; set; }


    }
}
