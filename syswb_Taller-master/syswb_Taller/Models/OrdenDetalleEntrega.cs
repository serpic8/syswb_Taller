using System;
using System.Collections.Generic;

namespace syswb_Taller.Models;

public partial class OrdenDetalleEntrega
{
    public int IdEntrega { get; set; }

    public int IdOrden { get; set; }

    public string RecibidoPor { get; set; } = null!;

    public string TelefonoRecibe { get; set; } = null!;

    public bool EsPropietario { get; set; }

    public int EntregadoPor { get; set; }

    public DateTime FechaEntrega { get; set; }

    public string? OrdenEntrega { get; set; }

    public int MecanicoResponsable { get; set; }

    public string? InformeTecnico { get; set; }

    public virtual Empleado EntregadoPorNavigation { get; set; } = null!;

    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    public virtual Empleado MecanicoResponsableNavigation { get; set; } = null!;
}
