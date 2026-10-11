using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("DetallePagoOrden")]
public partial class DetallePagoOrden
{
    [Key]
    [Column("idPagoOrden")]
    public int IdPagoOrden { get; set; }

    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("fechaPago", TypeName = "datetime")]
    public DateTime FechaPago { get; set; }

    [Column("montoPagado")]
    public double MontoPagado { get; set; }

    [Column("metodoPago")]
    [StringLength(50)]
    [Unicode(false)]
    public string MetodoPago { get; set; } = null!;

    [Column("observaciones")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Observaciones { get; set; }

    [Column("idEmpleado")]
    public int IdEmpleado { get; set; }

    [Column("idCliente")]
    public int IdCliente { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("DetallePagoOrdens")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    [ForeignKey("IdEmpleado")]
    [InverseProperty("DetallePagoOrdens")]
    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    [ForeignKey("IdOrden")]
    [InverseProperty("DetallePagoOrdens")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [InverseProperty("IdPagoOrdenNavigation")]
    public virtual ICollection<MovimientoFinanciero> MovimientoFinancieros { get; set; } = new List<MovimientoFinanciero>();
}
