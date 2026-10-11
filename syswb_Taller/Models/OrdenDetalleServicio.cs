using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("OrdenDetalle_Servicio")]
public partial class OrdenDetalleServicio
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("idServicio")]
    public int IdServicio { get; set; }

    [Column("opCantidad")]
    public int OpCantidad { get; set; }

    [Column("opPrecioUnit")]
    public double OpPrecioUnit { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenDetalleServicios")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdServicio")]
    [InverseProperty("OrdenDetalleServicios")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
