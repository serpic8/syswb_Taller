using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("PresupuestoDetalle_Servicio")]
public partial class PresupuestoDetalleServicio
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idPresupuesto")]
    public int IdPresupuesto { get; set; }

    [Column("idServicio")]
    public int IdServicio { get; set; }

    [Column("psCantidad")]
    public int PsCantidad { get; set; }

    [Column("psPrecioUnit")]
    public double PsPrecioUnit { get; set; }

    [ForeignKey("IdPresupuesto")]
    [InverseProperty("PresupuestoDetalleServicios")]
    public virtual Presupuesto IdPresupuestoNavigation { get; set; } = null!;

    [ForeignKey("IdServicio")]
    [InverseProperty("PresupuestoDetalleServicios")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
