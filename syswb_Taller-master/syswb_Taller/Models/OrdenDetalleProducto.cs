using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("OrdenDetalle_Producto")]
public partial class OrdenDetalleProducto
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("idProducto")]
    public int IdProducto { get; set; }

    [Column("opCantidad")]
    public int OpCantidad { get; set; }

    [Column("opPrecioUnit")]
    public double OpPrecioUnit { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenDetalleProductos")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdProducto")]
    [InverseProperty("OrdenDetalleProductos")]
    public virtual ProductoRepuesto IdProductoNavigation { get; set; } = null!;
}
