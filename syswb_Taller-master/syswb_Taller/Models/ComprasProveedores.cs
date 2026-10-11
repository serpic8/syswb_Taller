using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Compra_Proveedores")]
public partial class ComprasProveedores
{
    [Key]
    [Column("idCompra")]
    public int IdCompra { get; set; }

    [Column("compFecha", TypeName = "datetime")]
    public DateTime? CompFecha { get; set; }

    [Column("compTotal")]
    public double CompTotal { get; set; }

    [Column("cprActivo")]
    public bool CprActivo { get; set; }

    [InverseProperty("IdCompraNavigation")]
    public virtual ICollection<CompraProducto> CompraProductos { get; set; } = new List<CompraProducto>();

    [InverseProperty("IdCompraNavigation")]
    public virtual ICollection<MovimientoFinanciero> MovimientoFinancieros { get; set; } = new List<MovimientoFinanciero>();
}
