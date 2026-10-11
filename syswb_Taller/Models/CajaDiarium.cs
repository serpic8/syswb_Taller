using System;
using System.Collections.Generic;

namespace syswb_Taller.Models;

public partial class CajaDiarium
{
    public int IdCaja { get; set; }

    public DateOnly CjFecha { get; set; }

    public TimeOnly CjApertura { get; set; }

    public TimeOnly? CjCierre { get; set; }

    public virtual ICollection<MovimientoFinanciero> MovimientoFinancieros { get; set; } = new List<MovimientoFinanciero>();
}
