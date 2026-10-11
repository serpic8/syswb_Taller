using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("PresupuestoDetalle_Producto")]
public partial class PresupuestoDetalleProducto
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idPresupuesto")]
    public int IdPresupuesto { get; set; }

    [Column("idProducto")]
    public int IdProducto { get; set; }

    [Column("ppCantidad")]
    public int PpCantidad { get; set; }

    [Column("ppPrecioUnit")]
    public double PpPrecioUnit { get; set; }

    [ForeignKey("IdPresupuesto")]
    [InverseProperty("PresupuestoDetalleProductos")]
    public virtual Presupuesto IdPresupuestoNavigation { get; set; } = null!;

    [ForeignKey("IdProducto")]
    [InverseProperty("PresupuestoDetalleProductos")]
    public virtual ProductoRepuesto IdProductoNavigation { get; set; } = null!;
}
