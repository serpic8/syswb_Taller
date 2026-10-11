using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Producto_Repuesto")]
public partial class ProductoRepuesto
{
    [Key]
    [Column("idProducto")]
    public int IdProducto { get; set; }

    [Column("prNombre")]
    [StringLength(100)]
    [Unicode(false)]
    public string PrNombre { get; set; } = null!;

    [Column("prMarca")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PrMarca { get; set; }

    [Column("prCategoria")]
    [StringLength(100)]
    [Unicode(false)]
    public string? PrCategoria { get; set; }

    [Column("prCosto")]
    public double? PrCosto { get; set; }

    [Column("prPrecioVenta")]
    public double? PrPrecioVenta { get; set; }

    [Column("prStock")]
    public int? PrStock { get; set; }

    [Column("prActivo")]
    public bool PrActivo { get; set; }

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<CompraProducto> CompraProductos { get; set; } = new List<CompraProducto>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<DetallePackProducto> DetallePackProductos { get; set; } = new List<DetallePackProducto>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<OrdenDetalleProducto> OrdenDetalleProductos { get; set; } = new List<OrdenDetalleProducto>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<PresupuestoDetalleProducto> PresupuestoDetalleProductos { get; set; } = new List<PresupuestoDetalleProducto>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();
}
