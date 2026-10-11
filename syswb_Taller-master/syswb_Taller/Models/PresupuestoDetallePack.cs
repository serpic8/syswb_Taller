using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("PresupuestoDetalle_Pack")]
public partial class PresupuestoDetallePack
{
    [Key]
    [Column("idDetalle")]
    public int IdDetalle { get; set; }

    [Column("idPresupuesto")]
    public int IdPresupuesto { get; set; }

    [Column("idPack")]
    public int IdPack { get; set; }

    [Column("ppCantidad")]
    public int PpCantidad { get; set; }

    [Column("ppPrecioPack")]
    public double PpPrecioPack { get; set; }

    [ForeignKey("IdPack")]
    [InverseProperty("PresupuestoDetallePacks")]
    public virtual PackReparacion IdPackNavigation { get; set; } = null!;

    [ForeignKey("IdPresupuesto")]
    [InverseProperty("PresupuestoDetallePacks")]
    public virtual Presupuesto IdPresupuestoNavigation { get; set; } = null!;
}
