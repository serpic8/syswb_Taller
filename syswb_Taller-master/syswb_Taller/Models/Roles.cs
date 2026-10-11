using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

public partial class Roles
{
    [Key]
    [Column("idRol")]
    public int IdRol { get; set; }

    [Column("rTipo")]
    [StringLength(30)]
    [Unicode(false)]
    public string RTipo { get; set; } = null!;

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
