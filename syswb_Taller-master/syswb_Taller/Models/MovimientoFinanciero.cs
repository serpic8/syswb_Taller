using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("MovimientoFinanciero")]
public partial class MovimientoFinanciero
{
    [Key]
    [Column("idMovimiento")]
    public int IdMovimiento { get; set; }

    [Column("movFecha", TypeName = "datetime")]
    public DateTime MovFecha { get; set; }

    [Column("movMonto")]
    public double MovMonto { get; set; }

    [Column("mvTipoMovimiento")]
    [StringLength(50)]
    [Unicode(false)]
    public string MvTipoMovimiento { get; set; } = null!;

    [Column("idCaja")]
    public int IdCaja { get; set; }

    [Column("idPagoOrden")]
    public int? IdPagoOrden { get; set; }

    [Column("idCompra")]
    public int? IdCompra { get; set; }

    [ForeignKey("IdCaja")]
    [InverseProperty("MovimientoFinancieros")]
    public virtual CajaDiaria IdCajaNavigation { get; set; } = null!;

    [ForeignKey("IdCompra")]
    [InverseProperty("MovimientoFinancieros")]
    public virtual ComprasProveedores? IdCompraNavigation { get; set; }

    [ForeignKey("IdPagoOrden")]
    [InverseProperty("MovimientoFinancieros")]
    public virtual DetallePagoOrden? IdPagoOrdenNavigation { get; set; }
}
