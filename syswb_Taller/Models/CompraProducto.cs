using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[PrimaryKey("IdCompra", "IdProducto")]
[Table("Compra_Producto")]
public partial class CompraProducto
{
    [Column("cproCantidad")]
    public int CproCantidad { get; set; }

    [Column("cproPrecioU")]
    public double CproPrecioU { get; set; }

    [Key]
    [Column("idCompra")]
    public int IdCompra { get; set; }

    [Key]
    [Column("idProducto")]
    public int IdProducto { get; set; }

    [Column("idProveedor")]
    public int? IdProveedor { get; set; }

    [Column("cproActivo")]
    public bool CproActivo { get; set; }

    [ForeignKey("IdCompra")]
    [InverseProperty("CompraProductos")]
    public virtual ComprasProveedores IdCompraNavigation { get; set; } = null!;

    [ForeignKey("IdProducto")]
    [InverseProperty("CompraProductos")]
    public virtual ProductoRepuesto IdProductoNavigation { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    [InverseProperty("CompraProductos")]
    public virtual Proveedor? IdProveedorNavigation { get; set; }
}
