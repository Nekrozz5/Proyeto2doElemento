﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.DTOs.DetalleFactura
{
    public class DetalleFacturaDTO
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public string LibroTitulo { get; set; } = string.Empty;
        public decimal LibroPrecio { get; set; }
    }
}
