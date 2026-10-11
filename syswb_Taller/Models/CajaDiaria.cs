using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

public partial class CajaDiaria
{
    [Key]
    [Column("idCaja")]
    public int IdCaja { get; set; }

    [Column("cjFecha")]
    public DateOnly CjFecha { get; set; }

    [Column("cjApertura")]
    public TimeOnly CjApertura { get; set; }

    [Column("cjCierre")]
    public TimeOnly? CjCierre { get; set; }

    [InverseProperty("IdCajaNavigation")]
    public virtual ICollection<MovimientoFinanciero> MovimientoFinancieros { get; set; } = new List<MovimientoFinanciero>();
}
