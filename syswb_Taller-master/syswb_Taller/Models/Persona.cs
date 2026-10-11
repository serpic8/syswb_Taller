using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Persona")]
public partial class Persona
{
    [Key]
    [Column("idPersona")]
    public int IdPersona { get; set; }

    [Column("pNombre")]
    [StringLength(255)]
    [Unicode(false)]
    public string PNombre { get; set; } = null!;

    [Column("pApellido")]
    [StringLength(255)]
    [Unicode(false)]
    public string PApellido { get; set; } = null!;

    [Column("pTelefono")]
    [StringLength(20)]
    [Unicode(false)]
    public string PTelefono { get; set; } = null!;

    [Column("pCedula")]
    [StringLength(20)]
    [Unicode(false)]
    public string PCedula { get; set; } = null!;

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
