using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("OrdenDetalle_Entrega")]
public partial class OrdenDetalleEntrega
{
    [Key]
    [Column("idEntrega")]
    public int IdEntrega { get; set; }

    [Column("idOrden")]
    public int IdOrden { get; set; }

    [Column("recibido_por")]
    [StringLength(100)]
    [Unicode(false)]
    public string RecibidoPor { get; set; } = null!;

    [Column("telefono_recibe")]
    [StringLength(20)]
    [Unicode(false)]
    public string TelefonoRecibe { get; set; } = null!;

    [Column("es_propietario")]
    public bool EsPropietario { get; set; }

    [Column("entregado_por")]
    public int EntregadoPor { get; set; }

    [Column("fecha_entrega", TypeName = "datetime")]
    public DateTime FechaEntrega { get; set; }

    [Column("orden_entrega")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OrdenEntrega { get; set; }

    [Column("mecanico_responsable")]
    public int MecanicoResponsable { get; set; }

    [Column("informe_tecnico")]
    [Unicode(false)]
    public string? InformeTecnico { get; set; }

    [ForeignKey("EntregadoPor")]
    [InverseProperty("OrdenDetalleEntregaEntregadoPorNavigations")]
    public virtual Empleado EntregadoPorNavigation { get; set; } = null!;

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenDetalleEntregas")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("MecanicoResponsable")]
    [InverseProperty("OrdenDetalleEntregaMecanicoResponsableNavigations")]
    public virtual Empleado MecanicoResponsableNavigation { get; set; } = null!;
}
