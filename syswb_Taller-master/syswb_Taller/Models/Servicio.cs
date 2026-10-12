using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Servicio")]
public partial class Servicio
{
    [Key]
    [Column("idServicio")]
    public int IdServicio { get; set; }

    [Column("serNombre")]
    [StringLength(60)]
    [Unicode(false)]
    public string SerNombre { get; set; } = null!;

    [Column("serPrecioUnit")]
    public double SerPrecioUnit { get; set; }

    [Column("serActivo")]
    public bool SerActivo { get; set; }

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<DetallePackServicio> DetallePackServicios { get; set; } = new List<DetallePackServicio>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<OrdenDetalleServicio> OrdenDetalleServicios { get; set; } = new List<OrdenDetalleServicio>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<PresupuestoDetalleServicio> PresupuestoDetalleServicios { get; set; } = new List<PresupuestoDetalleServicio>();
}
