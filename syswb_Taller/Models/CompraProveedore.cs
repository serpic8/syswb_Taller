using System;
using System.Collections.Generic;

namespace syswb_Taller.Models;

public partial class CompraProveedore
{
    public int IdCompra { get; set; }

    public DateTime? CompFecha { get; set; }

    public double CompTotal { get; set; }

    public bool CprActivo { get; set; }

    public virtual ICollection<CompraProducto> CompraProductos { get; set; } = new List<CompraProducto>();

    public virtual ICollection<MovimientoFinanciero> MovimientoFinancieros { get; set; } = new List<MovimientoFinanciero>();
}
