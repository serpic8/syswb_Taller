using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("PackReparacion")]
public partial class PackReparacion
{
    [Key]
    [Column("idPack")]
    public int IdPack { get; set; }

    [Column("pckNombre")]
    [StringLength(60)]
    [Unicode(false)]
    public string PckNombre { get; set; } = null!;

    [Column("pckDescripcion")]
    [StringLength(250)]
    [Unicode(false)]
    public string? PckDescripcion { get; set; }

    [Column("pckPrecio")]
    public double? PckPrecio { get; set; }

    [Column("pckActivo")]
    public bool PckActivo { get; set; }

    [InverseProperty("IdPackNavigation")]
    public virtual ICollection<DetallePackProducto> DetallePackProductos { get; set; } = new List<DetallePackProducto>();

    [InverseProperty("IdPackNavigation")]
    public virtual ICollection<DetallePackServicio> DetallePackServicios { get; set; } = new List<DetallePackServicio>();

    [InverseProperty("IdPackNavigation")]
    public virtual ICollection<OrdenDetallePack> OrdenDetallePacks { get; set; } = new List<OrdenDetallePack>();

    [InverseProperty("IdPackNavigation")]
    public virtual ICollection<PresupuestoDetallePack> PresupuestoDetallePacks { get; set; } = new List<PresupuestoDetallePack>();
}
