using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("DetallePack_Servicio")]
public partial class DetallePackServicio
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("psCantidad")]
    public int PsCantidad { get; set; }

    [Column("psPrecioUnit")]
    public double? PsPrecioUnit { get; set; }

    [Column("idPack")]
    public int? IdPack { get; set; }

    [Column("idServicio")]
    public int? IdServicio { get; set; }

    [ForeignKey("IdPack")]
    [InverseProperty("DetallePackServicios")]
    public virtual PackReparacion? IdPackNavigation { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("DetallePackServicios")]
    public virtual Servicio? IdServicioNavigation { get; set; }
}
