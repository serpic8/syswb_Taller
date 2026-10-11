using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("OrdenDetalle_Pack")]
public partial class OrdenDetallePack
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("idPack")]
    public int IdPack { get; set; }

    [Column("opCantidad")]
    public int OpCantidad { get; set; }

    [Column("opPrecioPack")]
    public double OpPrecioPack { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenDetallePacks")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdPack")]
    [InverseProperty("OrdenDetallePacks")]
    public virtual PackReparacion IdPackNavigation { get; set; } = null!;
}
