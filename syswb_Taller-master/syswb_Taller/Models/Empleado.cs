using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Empleado")]
public partial class Empleado
{
    [Key]
    [Column("idEmpleado")]
    public int IdEmpleado { get; set; }

    [Column("empUsuario")]
    [StringLength(20)]
    [Unicode(false)]
    public string EmpUsuario { get; set; } = null!;

    [Column("idRol")]
    public int IdRol { get; set; }

    [Column("empContrasena")]
    public byte[]? EmpContrasena { get; set; }

    [Column("empActivo")]
    public bool EmpActivo { get; set; }

    [Column("idPersona")]
    public int IdPersona { get; set; }

    [Column("contrasena_texto")]
    [StringLength(128)]
    [Unicode(false)]
    public string? ContrasenaTexto { get; set; }

    [InverseProperty("IdEmpleadoNavigation")]
    public virtual ICollection<DetallePagoOrden> DetallePagoOrdens { get; set; } = new List<DetallePagoOrden>();

    [ForeignKey("IdPersona")]
    [InverseProperty("Empleados")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdRol")]
    [InverseProperty("Empleados")]
    public virtual Roles IdRolNavigation { get; set; } = null!;

    [InverseProperty("EntregadoPorNavigation")]
    public virtual ICollection<OrdenDetalleEntrega> OrdenDetalleEntregaEntregadoPorNavigations { get; set; } = new List<OrdenDetalleEntrega>();

    [InverseProperty("MecanicoResponsableNavigation")]
    public virtual ICollection<OrdenDetalleEntrega> OrdenDetalleEntregaMecanicoResponsableNavigations { get; set; } = new List<OrdenDetalleEntrega>();
}
