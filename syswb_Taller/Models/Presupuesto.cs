using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Presupuesto")]
public partial class Presupuesto
{
    [Key]
    [Column("idPresupuesto")]
    public int IdPresupuesto { get; set; }

    [Column("preFechaCreacion", TypeName = "datetime")]
    public DateTime PreFechaCreacion { get; set; }

    [Column("preEstado")]
    public bool PreEstado { get; set; }

    [Column("preCostoTotal")]
    public double PreCostoTotal { get; set; }

    [Column("preFalla")]
    [Unicode(false)]
    public string? PreFalla { get; set; }

    [Column("idVehiculo")]
    public int? IdVehiculo { get; set; }

    [Column("idCliente")]
    public int? IdCliente { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("Presupuestos")]
    public virtual Cliente? IdClienteNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Presupuestos")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [InverseProperty("IdPresupuestoNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdPresupuestoNavigation")]
    public virtual ICollection<PresupuestoDetallePack> PresupuestoDetallePacks { get; set; } = new List<PresupuestoDetallePack>();

    [InverseProperty("IdPresupuestoNavigation")]
    public virtual ICollection<PresupuestoDetalleProducto> PresupuestoDetalleProductos { get; set; } = new List<PresupuestoDetalleProducto>();

    [InverseProperty("IdPresupuestoNavigation")]
    public virtual ICollection<PresupuestoDetalleServicio> PresupuestoDetalleServicios { get; set; } = new List<PresupuestoDetalleServicio>();
}
