using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[PrimaryKey("IdProveedor", "IdProducto")]
[Table("ProveedorProducto")]
public partial class ProveedorProducto
{
    [Key]
    [Column("idProveedor")]
    public int IdProveedor { get; set; }

    [Key]
    [Column("idProducto")]
    public int IdProducto { get; set; }

    [Column("precioRef")]
    public double PrecioRef { get; set; }

    [Column("activo")]
    public bool Activo { get; set; }

    [ForeignKey("IdProducto")]
    [InverseProperty("ProveedorProductos")]
    public virtual ProductoRepuesto IdProductoNavigation { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    [InverseProperty("ProveedorProductos")]
    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
