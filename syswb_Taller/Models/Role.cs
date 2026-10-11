using System;
using System.Collections.Generic;

namespace syswb_Taller.Models;

public partial class Role
{
    public int IdRol { get; set; }

    public string RTipo { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
