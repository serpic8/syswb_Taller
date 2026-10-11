using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

[Table("Vehiculo")]
public partial class Vehiculo
{
    [Key]
    [Column("idVehiculo")]
    public int IdVehiculo { get; set; }

    [Column("vhMatricula")]
    [StringLength(60)]
    [Unicode(false)]
    public string VhMatricula { get; set; } = null!;

    [Column("vhMarca")]
    [StringLength(60)]
    [Unicode(false)]
    public string VhMarca { get; set; } = null!;

    [Column("vhModelo")]
    [StringLength(60)]
    [Unicode(false)]
    public string VhModelo { get; set; } = null!;

    [Column("vhAño")]
    public int VhAño { get; set; }

    [Column("vhColor")]
    [StringLength(60)]
    [Unicode(false)]
    public string VhColor { get; set; } = null!;

    [Column("vhEstado")]
    [StringLength(50)]
    [Unicode(false)]
    public string VhEstado { get; set; } = null!;

    [Column("idCliente")]
    public int IdCliente { get; set; }

    [Column("vhActivo")]
    public bool? VhActivo { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("Vehiculos")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}
