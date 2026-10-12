using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Models;

public partial class BdTallerContext : DbContext
{
    public BdTallerContext()
    {
    }

    public BdTallerContext(DbContextOptions<BdTallerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CajaDiaria> CajaDiaria { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<CompraProducto> CompraProductos { get; set; }

    public virtual DbSet<ComprasProveedores> CompraProveedores { get; set; }

    public virtual DbSet<DetallePackProducto> DetallePackProductos { get; set; }

    public virtual DbSet<DetallePackServicio> DetallePackServicios { get; set; }

    public virtual DbSet<DetallePagoOrden> DetallePagoOrdens { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<MovimientoFinanciero> MovimientoFinancieros { get; set; }

    public virtual DbSet<Orden> Ordenes { get; set; }

    public virtual DbSet<OrdenDetalleEntrega> OrdenDetalleEntregas { get; set; }

    public virtual DbSet<OrdenDetallePack> OrdenDetallePacks { get; set; }

    public virtual DbSet<OrdenDetalleProducto> OrdenDetalleProductos { get; set; }

    public virtual DbSet<OrdenDetalleServicio> OrdenDetalleServicios { get; set; }

    public virtual DbSet<PackReparacion> PackReparaciones { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Presupuesto> Presupuestos { get; set; }

    public virtual DbSet<PresupuestoDetallePack> PresupuestoDetallePacks { get; set; }

    public virtual DbSet<PresupuestoDetalleProducto> PresupuestoDetalleProductos { get; set; }

    public virtual DbSet<PresupuestoDetalleServicio> PresupuestoDetalleServicios { get; set; }

    public virtual DbSet<ProductoRepuesto> ProductoRepuestos { get; set; }

    public virtual DbSet<Proveedor> Proveedores { get; set; }

    public virtual DbSet<ProveedorProducto> ProveedorProductos { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data source=DESKTOP-C16QH16\\SQL_SERVER;initial catalog=BD_Taller; trusted_connection=yes; Encrypt=False; TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CajaDiaria>(entity =>
        {
            entity.HasKey(e => e.IdCaja).HasName("PK__CajaDiar__8BC79B34BDBB6000");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__885457EEF2B3EC69");

            entity.Property(e => e.ClActivo).HasDefaultValue(true);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Clientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cliente__idPerso__412EB0B6");
        });

        modelBuilder.Entity<CompraProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdCompra, e.IdProducto }).HasName("PK__Compra_P__78C6D7A419E33D0D");

            entity.ToTable("Compra_Producto", tb =>
                {
                    tb.HasTrigger("trg_RecalculaCompTotal_After_CompraProducto");
                    tb.HasTrigger("trg_RecalculaStock_SoftDeleteCompraProducto");
                    tb.HasTrigger("trg_Stock_CompraProducto_Delete");
                    tb.HasTrigger("trg_Stock_CompraProducto_Insert");
                });

            entity.Property(e => e.CproActivo).HasDefaultValue(true);

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.CompraProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra_Pr__idCom__4F7CD00D");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.CompraProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra_Pr__idPro__5070F446");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.CompraProductos).HasConstraintName("FK_Compra_Producto_Proveedor");
        });

        modelBuilder.Entity<ComprasProveedores>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra_P__48B99DB7804B7932");

            entity.ToTable("Compra_Proveedores", tb =>
                {
                    tb.HasTrigger("trg_ActualizaMovimiento_After_Upd_CompraProv");
                    tb.HasTrigger("trg_CreaMovimiento_After_CompraProv");
                    tb.HasTrigger("trg_SoftDeleteCompraProveedor");
                });

            entity.Property(e => e.CprActivo).HasDefaultValue(true);
        });

        modelBuilder.Entity<DetallePackProducto>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DetalleP__49CAE2FB668C5957");

            entity.ToTable("DetallePack_Producto", tb => tb.HasTrigger("trg_RecalculaPrecio_After_DetallePackProducto"));

            entity.Property(e => e.PdCantidad).HasDefaultValue(1);

            entity.HasOne(d => d.IdPackNavigation).WithMany(p => p.DetallePackProductos).HasConstraintName("FK__DetallePa__idPac__656C112C");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallePackProductos).HasConstraintName("FK__DetallePa__idPro__66603565");
        });

        modelBuilder.Entity<DetallePackServicio>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DetalleP__49CAE2FB36000B38");

            entity.ToTable("DetallePack_Servicio", tb => tb.HasTrigger("trg_RecalculaPrecio_After_DetallePackServicio"));

            entity.Property(e => e.PsCantidad).HasDefaultValue(1);

            entity.HasOne(d => d.IdPackNavigation).WithMany(p => p.DetallePackServicios).HasConstraintName("FK__DetallePa__idPac__60A75C0F");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.DetallePackServicios).HasConstraintName("FK__DetallePa__idSer__619B8048");
        });

        modelBuilder.Entity<DetallePagoOrden>(entity =>
        {
            entity.HasKey(e => e.IdPagoOrden).HasName("PK__DetalleP__B3EC2D27A35AB3C5");

            entity.ToTable("DetallePagoOrden", tb => tb.HasTrigger("trg_CreaMovimiento_After_DetallePagoOrden"));

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.DetallePagoOrdens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePa__idCli__0D7A0286");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.DetallePagoOrdens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePa__idEmp__0C85DE4D");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetallePagoOrdens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetallePa__idOrd__0B91BA14");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__Empleado__5295297C3DDA20F8");

            entity.ToTable("Empleado", tb => tb.HasTrigger("trg_Empleados_Cifrar"));

            entity.Property(e => e.EmpActivo).HasDefaultValue(true);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Empleados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Empleado__idPers__3D5E1FD2");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Empleados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Empleado__idRol__3B75D760");
        });

        modelBuilder.Entity<MovimientoFinanciero>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimien__628521731729127C");

            entity.HasOne(d => d.IdCajaNavigation).WithMany(p => p.MovimientoFinancieros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movimient__idCaj__10566F31");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.MovimientoFinancieros).HasConstraintName("FK__Movimient__idCom__123EB7A3");

            entity.HasOne(d => d.IdPagoOrdenNavigation).WithMany(p => p.MovimientoFinancieros).HasConstraintName("FK__Movimient__idPag__114A936A");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__Orden__C8AAF6F325C7D707");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Ordens).HasConstraintName("FK__Orden__idCliente__75A278F5");

            entity.HasOne(d => d.IdPresupuestoNavigation).WithMany(p => p.Ordens).HasConstraintName("FK__Orden__idPresupu__778AC167");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Ordens).HasConstraintName("FK__Orden__idVehicul__76969D2E");
        });

        modelBuilder.Entity<OrdenDetalleEntrega>(entity =>
        {
            entity.ToTable("OrdenDetalle_Entrega");

            entity.HasKey(e => e.IdEntrega).HasName("PK__OrdenDet__6CA5A988...");

            entity.Property(e => e.IdEntrega).HasColumnName("idEntrega");
            entity.Property(e => e.IdOrden).HasColumnName("idOrden");
            entity.Property(e => e.RecibidoPor).HasColumnName("recibido_por");
            entity.Property(e => e.TelefonoRecibe).HasColumnName("telefono_recibe");
            entity.Property(e => e.EsPropietario).HasColumnName("es_propietario");
            entity.Property(e => e.EntregadoPor).HasColumnName("entregado_por");
            entity.Property(e => e.FechaEntrega).HasColumnName("fecha_entrega");
            entity.Property(e => e.OrdenEntrega).HasColumnName("orden_entrega");
            entity.Property(e => e.MecanicoResponsable).HasColumnName("mecanico_responsable");
            entity.Property(e => e.InformeTecnico).HasColumnName("informe_tecnico");

            entity.HasOne(d => d.EntregadoPorNavigation)
                .WithMany(p => p.OrdenDetalleEntregaEntregadoPorNavigations)
                .HasForeignKey(d => d.EntregadoPor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__entre__...");

            entity.HasOne(d => d.IdOrdenNavigation)
                .WithMany(p => p.OrdenDetalleEntregas)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idOrd__...");

            entity.HasOne(d => d.MecanicoResponsableNavigation)
                .WithMany(p => p.OrdenDetalleEntregaMecanicoResponsableNavigations)
                .HasForeignKey(d => d.MecanicoResponsable)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__mecan__...");
        });

        modelBuilder.Entity<OrdenDetallePack>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__OrdenDet__49CAE2FB2778F03E");

            entity.ToTable("OrdenDetalle_Pack", tb =>
                {
                    tb.HasTrigger("trg_RecalculaCosto_After_OrdenDetPack");
                    tb.HasTrigger("trg_Stock_OrdenDetPack_Delete");
                    tb.HasTrigger("trg_Stock_OrdenDetPack_Insert");
                });

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenDetallePacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idOrd__06CD04F7");

            entity.HasOne(d => d.IdPackNavigation).WithMany(p => p.OrdenDetallePacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idPac__07C12930");
        });

        modelBuilder.Entity<OrdenDetalleProducto>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__OrdenDet__49CAE2FBA933A025");

            entity.ToTable("OrdenDetalle_Producto", tb =>
                {
                    tb.HasTrigger("trg_RecalculaCosto_After_OrdenDetProducto");
                    tb.HasTrigger("trg_Stock_OrdenDetProd_Delete");
                    tb.HasTrigger("trg_Stock_OrdenDetProd_Insert");
                });

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenDetalleProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idOrd__02FC7413");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.OrdenDetalleProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idPro__03F0984C");
        });

        modelBuilder.Entity<OrdenDetalleServicio>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__OrdenDet__49CAE2FB909CADDA");

            entity.ToTable("OrdenDetalle_Servicio", tb => tb.HasTrigger("trg_RecalculaCosto_After_OrdenDetServicio"));

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idOrd__7F2BE32F");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.OrdenDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrdenDeta__idSer__00200768");
        });

        modelBuilder.Entity<PackReparacion>(entity =>
        {
            entity.HasKey(e => e.IdPack).HasName("PK__PackRepa__BD2236282839AC60");

            entity.Property(e => e.PckActivo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__Persona__A4788141B3E17108");
        });

        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.HasKey(e => e.IdPresupuesto).HasName("PK__Presupue__99893A4E78E9F079");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Presupuestos).HasConstraintName("FK__Presupues__idCli__571DF1D5");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Presupuestos).HasConstraintName("FK__Presupues__idVeh__5629CD9C");
        });

        modelBuilder.Entity<PresupuestoDetallePack>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Presupue__49CAE2FBB5B017CA");

            entity.ToTable("PresupuestoDetalle_Pack", tb => tb.HasTrigger("trg_RecalculaCosto_After_PresupuestoDetPack"));

            entity.HasOne(d => d.IdPackNavigation).WithMany(p => p.PresupuestoDetallePacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idPac__72C60C4A");

            entity.HasOne(d => d.IdPresupuestoNavigation).WithMany(p => p.PresupuestoDetallePacks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idPre__71D1E811");
        });

        modelBuilder.Entity<PresupuestoDetalleProducto>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Presupue__49CAE2FBB0EEB09D");

            entity.ToTable("PresupuestoDetalle_Producto", tb => tb.HasTrigger("trg_RecalculaCosto_After_PresupuestoDetProducto"));

            entity.HasOne(d => d.IdPresupuestoNavigation).WithMany(p => p.PresupuestoDetalleProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idPre__6E01572D");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PresupuestoDetalleProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idPro__6EF57B66");
        });

        modelBuilder.Entity<PresupuestoDetalleServicio>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Presupue__49CAE2FB38BEC40A");

            entity.ToTable("PresupuestoDetalle_Servicio", tb => tb.HasTrigger("trg_RecalculaCosto_After_PresupuestoDetServicio"));

            entity.HasOne(d => d.IdPresupuestoNavigation).WithMany(p => p.PresupuestoDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idPre__6A30C649");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.PresupuestoDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Presupues__idSer__6B24EA82");
        });

        modelBuilder.Entity<ProductoRepuesto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__07F4A1324BB0A457");

            entity.Property(e => e.PrActivo).HasDefaultValue(true);
            entity.Property(e => e.PrStock).HasDefaultValue(0);
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__A3FA8E6B0D4767F7");

            entity.Property(e => e.PActivo).HasDefaultValue(true);
        });

        modelBuilder.Entity<ProveedorProducto>(entity =>
        {
            entity.HasKey(e => new { e.IdProveedor, e.IdProducto }).HasName("PK__Proveedo__9385C47876B3D8EB");

            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProveedorProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProveedorProducto_Producto");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.ProveedorProductos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProveedorProducto_Proveedor");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__3C872F761B0B34BE");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.IdServicio).HasName("PK__Servicio__CEB981199631AA76");

            entity.Property(e => e.SerActivo).HasDefaultValue(true);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo).HasName("PK__Vehiculo__4868297022A6F9CA");

            entity.Property(e => e.VhActivo).HasDefaultValue(true);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Vehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehiculo__idClie__440B1D61");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
