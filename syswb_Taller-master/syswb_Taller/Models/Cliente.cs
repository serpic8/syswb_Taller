using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Cliente")]
public partial class Cliente
{
    [Key]
    [Column("idCliente")]
    public int IdCliente { get; set; }

    [Column("clEmail")]
    [StringLength(150)]
    [Unicode(false)]
    public string ClEmail { get; set; } = null!;

    [Column("clActivo")]
    public bool ClActivo { get; set; }

    [Column("idPersona")]
    public int IdPersona { get; set; }

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<DetallePagoOrden> DetallePagoOrdens { get; set; } = new List<DetallePagoOrden>();

    [ForeignKey("IdPersona")]
    [InverseProperty("Clientes")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
