using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Orden")]
public partial class Orden
{
    [Key]
    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("orFechaCreacion", TypeName = "datetime")]
    public DateTime OrFechaCreacion { get; set; }

    [Column("orEstado")]
    [StringLength(30)]
    [Unicode(false)]
    public string OrEstado { get; set; } = null!;

    [Column("orCostoTotal")]
    public double OrCostoTotal { get; set; }

    [Column("orTipo")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OrTipo { get; set; }

    [Column("orFechaEntrega", TypeName = "datetime")]
    public DateTime? OrFechaEntrega { get; set; }

    [Column("orFechaFin", TypeName = "datetime")]
    public DateTime? OrFechaFin { get; set; }

    [Column("orFalla")]
    [Unicode(false)]
    public string OrFalla { get; set; } = null!;

    [Column("idCliente")]
    public int? IdCliente { get; set; }

    [Column("idVehiculo")]
    public int? IdVehiculo { get; set; }

    [Column("idPresupuesto")]
    public int? IdPresupuesto { get; set; }

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<DetallePagoOrden> DetallePagoOrdens { get; set; } = new List<DetallePagoOrden>();

    [ForeignKey("IdCliente")]
    [InverseProperty("Ordens")]
    public virtual Cliente? IdClienteNavigation { get; set; }

    [ForeignKey("IdPresupuesto")]
    [InverseProperty("Ordens")]
    public virtual Presupuesto? IdPresupuestoNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Ordens")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenDetalleEntrega> OrdenDetalleEntregas { get; set; } = new List<OrdenDetalleEntrega>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenDetallePack> OrdenDetallePacks { get; set; } = new List<OrdenDetallePack>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenDetalleProducto> OrdenDetalleProductos { get; set; } = new List<OrdenDetalleProducto>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenDetalleServicio> OrdenDetalleServicios { get; set; } = new List<OrdenDetalleServicio>();
}
