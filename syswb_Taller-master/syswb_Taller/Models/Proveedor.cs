using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Proveedor")]
public partial class Proveedor
{
    [Key]
    [Column("idProveedor")]
    public int IdProveedor { get; set; }

    [Column("pNombre")]
    [StringLength(100)]
    [Unicode(false)]
    public string PNombre { get; set; } = null!;

    [Column("pTelefono")]
    [StringLength(20)]
    [Unicode(false)]
    public string PTelefono { get; set; } = null!;

    [Column("pActivo")]
    public bool PActivo { get; set; }

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<CompraProducto> CompraProductos { get; set; } = new List<CompraProducto>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<ProveedorProducto> ProveedorProductos { get; set; } = new List<ProveedorProducto>();
}
