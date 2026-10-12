using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("DetallePack_Producto")]
public partial class DetallePackProducto
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("pdCantidad")]
    public int PdCantidad { get; set; }

    [Column("pdPrecioUnit")]
    public double? PdPrecioUnit { get; set; }

    [Column("idPack")]
    public int? IdPack { get; set; }

    [Column("idProducto")]
    public int? IdProducto { get; set; }

    [ForeignKey("IdPack")]
    [InverseProperty("DetallePackProductos")]
    public virtual PackReparacion? IdPackNavigation { get; set; }

    [ForeignKey("IdProducto")]
    [InverseProperty("DetallePackProductos")]
    public virtual ProductoRepuesto? IdProductoNavigation { get; set; }
}
