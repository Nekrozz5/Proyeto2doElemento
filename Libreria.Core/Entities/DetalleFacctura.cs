using Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DetalleFactura
{
    public int Id { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;

    public int FacturaId { get; set; }
    public virtual Factura Factura { get; set; } = null!;

    public int LibroId { get; set; }
    public virtual Libro Libro { get; set; } = null!;
}
