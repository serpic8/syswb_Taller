USE [master]
GO
/****** Object:  Database [BD_Taller]    Script Date: 20/5/2026 11:00:35 ******/
CREATE DATABASE [BD_Taller]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BD_Taller', FILENAME = N'C:\Program Files\Microsoft SQL Server\Data\BD_Taller.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BD_Taller_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\Data\BD_Taller_log.ldf' , SIZE = 335872KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [BD_Taller] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BD_Taller].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BD_Taller] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BD_Taller] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BD_Taller] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BD_Taller] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BD_Taller] SET ARITHABORT OFF 
GO
ALTER DATABASE [BD_Taller] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BD_Taller] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BD_Taller] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BD_Taller] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BD_Taller] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BD_Taller] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BD_Taller] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BD_Taller] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BD_Taller] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BD_Taller] SET  ENABLE_BROKER 
GO
ALTER DATABASE [BD_Taller] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BD_Taller] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BD_Taller] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BD_Taller] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BD_Taller] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BD_Taller] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BD_Taller] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BD_Taller] SET RECOVERY FULL 
GO
ALTER DATABASE [BD_Taller] SET  MULTI_USER 
GO
ALTER DATABASE [BD_Taller] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BD_Taller] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BD_Taller] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BD_Taller] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BD_Taller] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BD_Taller] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'BD_Taller', N'ON'
GO
ALTER DATABASE [BD_Taller] SET QUERY_STORE = ON
GO
ALTER DATABASE [BD_Taller] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [BD_Taller]
GO
/****** Object:  User [serpic8]    Script Date: 20/5/2026 11:00:35 ******/
CREATE USER [serpic8] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [alee]    Script Date: 20/5/2026 11:00:35 ******/
CREATE USER [alee] FOR LOGIN [alee] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [serpic8]
GO
ALTER ROLE [db_owner] ADD MEMBER [alee]
GO
/****** Object:  UserDefinedFunction [dbo].[DecryptPassword]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   FUNCTION [dbo].[DecryptPassword](@ciphertext VARBINARY(MAX))
RETURNS VARCHAR(128)
AS
BEGIN
  RETURN CONVERT(VARCHAR(128), DECRYPTBYPASSPHRASE('Password', @ciphertext));
END;
GO
/****** Object:  Table [dbo].[CajaDiaria]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CajaDiaria](
	[idCaja] [int] IDENTITY(1,1) NOT NULL,
	[cjFecha] [date] NOT NULL,
	[cjApertura] [time](7) NOT NULL,
	[cjCierre] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[idCaja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[idCliente] [int] IDENTITY(1,1) NOT NULL,
	[clEmail] [varchar](150) NOT NULL,
	[clActivo] [bit] NOT NULL,
	[idPersona] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compra_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Compra_Producto](
	[cproCantidad] [int] NOT NULL,
	[cproPrecioU] [float] NOT NULL,
	[idCompra] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[idProveedor] [int] NULL,
	[cproActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idCompra] ASC,
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compra_Proveedores]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Compra_Proveedores](
	[idCompra] [int] IDENTITY(1,1) NOT NULL,
	[compFecha] [datetime] NULL,
	[compTotal] [float] NOT NULL,
	[cprActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idCompra] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallePack_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePack_Producto](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[pdCantidad] [int] NOT NULL,
	[pdPrecioUnit] [float] NULL,
	[idPack] [int] NULL,
	[idProducto] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallePack_Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePack_Servicio](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[psCantidad] [int] NOT NULL,
	[psPrecioUnit] [float] NULL,
	[idPack] [int] NULL,
	[idServicio] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallePagoOrden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePagoOrden](
	[idPagoOrden] [int] IDENTITY(1,1) NOT NULL,
	[idOrden] [int] NOT NULL,
	[fechaPago] [datetime] NOT NULL,
	[montoPagado] [float] NOT NULL,
	[metodoPago] [varchar](50) NOT NULL,
	[observaciones] [varchar](255) NULL,
	[idEmpleado] [int] NOT NULL,
	[idCliente] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idPagoOrden] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Empleado]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Empleado](
	[idEmpleado] [int] IDENTITY(1,1) NOT NULL,
	[empUsuario] [varchar](20) NOT NULL,
	[idRol] [int] NOT NULL,
	[empContrasena] [varbinary](max) NULL,
	[empActivo] [bit] NOT NULL,
	[idPersona] [int] NOT NULL,
	[contrasena_texto] [varchar](128) NULL,
PRIMARY KEY CLUSTERED 
(
	[idEmpleado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MovimientoFinanciero]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MovimientoFinanciero](
	[idMovimiento] [int] IDENTITY(1,1) NOT NULL,
	[movFecha] [datetime] NOT NULL,
	[movMonto] [float] NOT NULL,
	[mvTipoMovimiento] [varchar](50) NOT NULL,
	[idCaja] [int] NOT NULL,
	[idPagoOrden] [int] NULL,
	[idCompra] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idMovimiento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orden](
	[idOrden] [int] IDENTITY(1,1) NOT NULL,
	[orFechaCreacion] [datetime] NOT NULL,
	[orEstado] [varchar](30) NOT NULL,
	[orCostoTotal] [float] NOT NULL,
	[orTipo] [varchar](50) NULL,
	[orFechaEntrega] [datetime] NULL,
	[orFechaFin] [datetime] NULL,
	[orFalla] [varchar](max) NOT NULL,
	[idCliente] [int] NULL,
	[idVehiculo] [int] NULL,
	[idPresupuesto] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idOrden] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrdenDetalle_Entrega]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdenDetalle_Entrega](
	[idEntrega] [int] IDENTITY(1,1) NOT NULL,
	[idOrden] [int] NOT NULL,
	[recibido_por] [varchar](100) NOT NULL,
	[telefono_recibe] [varchar](20) NOT NULL,
	[es_propietario] [bit] NOT NULL,
	[entregado_por] [int] NOT NULL,
	[fecha_entrega] [datetime] NOT NULL,
	[orden_entrega] [varchar](50) NULL,
	[mecanico_responsable] [int] NOT NULL,
	[informe_tecnico] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[idEntrega] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrdenDetalle_Pack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdenDetalle_Pack](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idOrden] [int] NOT NULL,
	[idPack] [int] NOT NULL,
	[opCantidad] [int] NOT NULL,
	[opPrecioPack] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrdenDetalle_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdenDetalle_Producto](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idOrden] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[opCantidad] [int] NOT NULL,
	[opPrecioUnit] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrdenDetalle_Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrdenDetalle_Servicio](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idOrden] [int] NOT NULL,
	[idServicio] [int] NOT NULL,
	[opCantidad] [int] NOT NULL,
	[opPrecioUnit] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PackReparacion]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackReparacion](
	[idPack] [int] IDENTITY(1,1) NOT NULL,
	[pckNombre] [varchar](60) NOT NULL,
	[pckDescripcion] [varchar](250) NULL,
	[pckPrecio] [float] NULL,
	[pckActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idPack] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Persona]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persona](
	[idPersona] [int] IDENTITY(1,1) NOT NULL,
	[pNombre] [varchar](255) NOT NULL,
	[pApellido] [varchar](255) NOT NULL,
	[pTelefono] [varchar](20) NOT NULL,
	[pCedula] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idPersona] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Presupuesto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Presupuesto](
	[idPresupuesto] [int] IDENTITY(1,1) NOT NULL,
	[preFechaCreacion] [datetime] NOT NULL,
	[preEstado] [bit] NOT NULL,
	[preCostoTotal] [float] NOT NULL,
	[preFalla] [varchar](max) NULL,
	[idVehiculo] [int] NULL,
	[idCliente] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[idPresupuesto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PresupuestoDetalle_Pack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PresupuestoDetalle_Pack](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idPresupuesto] [int] NOT NULL,
	[idPack] [int] NOT NULL,
	[ppCantidad] [int] NOT NULL,
	[ppPrecioPack] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PresupuestoDetalle_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PresupuestoDetalle_Producto](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idPresupuesto] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[ppCantidad] [int] NOT NULL,
	[ppPrecioUnit] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PresupuestoDetalle_Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PresupuestoDetalle_Servicio](
	[idDetalle] [int] IDENTITY(1,1) NOT NULL,
	[idPresupuesto] [int] NOT NULL,
	[idServicio] [int] NOT NULL,
	[psCantidad] [int] NOT NULL,
	[psPrecioUnit] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Producto_Repuesto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto_Repuesto](
	[idProducto] [int] IDENTITY(1,1) NOT NULL,
	[prNombre] [varchar](100) NOT NULL,
	[prMarca] [varchar](100) NULL,
	[prCategoria] [varchar](100) NULL,
	[prCosto] [float] NULL,
	[prPrecioVenta] [float] NULL,
	[prStock] [int] NULL,
	[prActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedor](
	[idProveedor] [int] IDENTITY(1,1) NOT NULL,
	[pNombre] [varchar](100) NOT NULL,
	[pTelefono] [varchar](20) NOT NULL,
	[pActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idProveedor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProveedorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProveedorProducto](
	[idProveedor] [int] NOT NULL,
	[idProducto] [int] NOT NULL,
	[precioRef] [float] NOT NULL,
	[activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idProveedor] ASC,
	[idProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[idRol] [int] IDENTITY(1,1) NOT NULL,
	[rTipo] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idRol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Servicio](
	[idServicio] [int] IDENTITY(1,1) NOT NULL,
	[serNombre] [varchar](60) NOT NULL,
	[serPrecioUnit] [float] NOT NULL,
	[serActivo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[idServicio] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Vehiculo]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vehiculo](
	[idVehiculo] [int] IDENTITY(1,1) NOT NULL,
	[vhMatricula] [varchar](60) NOT NULL,
	[vhMarca] [varchar](60) NOT NULL,
	[vhModelo] [varchar](60) NOT NULL,
	[vhAño] [int] NOT NULL,
	[vhColor] [varchar](60) NOT NULL,
	[vhEstado] [varchar](50) NOT NULL,
	[idCliente] [int] NOT NULL,
	[vhActivo] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[idVehiculo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cliente] ADD  DEFAULT ((1)) FOR [clActivo]
GO
ALTER TABLE [dbo].[Compra_Producto] ADD  DEFAULT ((1)) FOR [cproActivo]
GO
ALTER TABLE [dbo].[Compra_Proveedores] ADD  CONSTRAINT [DF_Compra_Proveedores_compTotal]  DEFAULT ((0)) FOR [compTotal]
GO
ALTER TABLE [dbo].[Compra_Proveedores] ADD  DEFAULT ((1)) FOR [cprActivo]
GO
ALTER TABLE [dbo].[DetallePack_Producto] ADD  DEFAULT ((1)) FOR [pdCantidad]
GO
ALTER TABLE [dbo].[DetallePack_Servicio] ADD  DEFAULT ((1)) FOR [psCantidad]
GO
ALTER TABLE [dbo].[Empleado] ADD  DEFAULT ((1)) FOR [empActivo]
GO
ALTER TABLE [dbo].[PackReparacion] ADD  DEFAULT ((1)) FOR [pckActivo]
GO
ALTER TABLE [dbo].[Presupuesto] ADD  DEFAULT ((0)) FOR [preEstado]
GO
ALTER TABLE [dbo].[Producto_Repuesto] ADD  DEFAULT ((0)) FOR [prStock]
GO
ALTER TABLE [dbo].[Producto_Repuesto] ADD  DEFAULT ((1)) FOR [prActivo]
GO
ALTER TABLE [dbo].[Proveedor] ADD  DEFAULT ((1)) FOR [pActivo]
GO
ALTER TABLE [dbo].[ProveedorProducto] ADD  DEFAULT ((1)) FOR [activo]
GO
ALTER TABLE [dbo].[Servicio] ADD  DEFAULT ((1)) FOR [serActivo]
GO
ALTER TABLE [dbo].[Vehiculo] ADD  DEFAULT ((1)) FOR [vhActivo]
GO
ALTER TABLE [dbo].[Cliente]  WITH CHECK ADD FOREIGN KEY([idPersona])
REFERENCES [dbo].[Persona] ([idPersona])
GO
ALTER TABLE [dbo].[Compra_Producto]  WITH CHECK ADD FOREIGN KEY([idCompra])
REFERENCES [dbo].[Compra_Proveedores] ([idCompra])
GO
ALTER TABLE [dbo].[Compra_Producto]  WITH CHECK ADD FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto_Repuesto] ([idProducto])
GO
ALTER TABLE [dbo].[Compra_Producto]  WITH CHECK ADD  CONSTRAINT [FK_Compra_Producto_Proveedor] FOREIGN KEY([idProveedor])
REFERENCES [dbo].[Proveedor] ([idProveedor])
GO
ALTER TABLE [dbo].[Compra_Producto] CHECK CONSTRAINT [FK_Compra_Producto_Proveedor]
GO
ALTER TABLE [dbo].[DetallePack_Producto]  WITH CHECK ADD FOREIGN KEY([idPack])
REFERENCES [dbo].[PackReparacion] ([idPack])
GO
ALTER TABLE [dbo].[DetallePack_Producto]  WITH CHECK ADD FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto_Repuesto] ([idProducto])
GO
ALTER TABLE [dbo].[DetallePack_Servicio]  WITH CHECK ADD FOREIGN KEY([idPack])
REFERENCES [dbo].[PackReparacion] ([idPack])
GO
ALTER TABLE [dbo].[DetallePack_Servicio]  WITH CHECK ADD FOREIGN KEY([idServicio])
REFERENCES [dbo].[Servicio] ([idServicio])
GO
ALTER TABLE [dbo].[DetallePagoOrden]  WITH CHECK ADD FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[DetallePagoOrden]  WITH CHECK ADD FOREIGN KEY([idEmpleado])
REFERENCES [dbo].[Empleado] ([idEmpleado])
GO
ALTER TABLE [dbo].[DetallePagoOrden]  WITH CHECK ADD FOREIGN KEY([idOrden])
REFERENCES [dbo].[Orden] ([idOrden])
GO
ALTER TABLE [dbo].[Empleado]  WITH CHECK ADD FOREIGN KEY([idPersona])
REFERENCES [dbo].[Persona] ([idPersona])
GO
ALTER TABLE [dbo].[Empleado]  WITH CHECK ADD FOREIGN KEY([idRol])
REFERENCES [dbo].[Roles] ([idRol])
GO
ALTER TABLE [dbo].[MovimientoFinanciero]  WITH CHECK ADD FOREIGN KEY([idCaja])
REFERENCES [dbo].[CajaDiaria] ([idCaja])
GO
ALTER TABLE [dbo].[MovimientoFinanciero]  WITH CHECK ADD FOREIGN KEY([idCompra])
REFERENCES [dbo].[Compra_Proveedores] ([idCompra])
GO
ALTER TABLE [dbo].[MovimientoFinanciero]  WITH CHECK ADD FOREIGN KEY([idPagoOrden])
REFERENCES [dbo].[DetallePagoOrden] ([idPagoOrden])
GO
ALTER TABLE [dbo].[Orden]  WITH CHECK ADD FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[Orden]  WITH CHECK ADD FOREIGN KEY([idPresupuesto])
REFERENCES [dbo].[Presupuesto] ([idPresupuesto])
GO
ALTER TABLE [dbo].[Orden]  WITH CHECK ADD FOREIGN KEY([idVehiculo])
REFERENCES [dbo].[Vehiculo] ([idVehiculo])
GO
ALTER TABLE [dbo].[OrdenDetalle_Entrega]  WITH CHECK ADD FOREIGN KEY([entregado_por])
REFERENCES [dbo].[Empleado] ([idEmpleado])
GO
ALTER TABLE [dbo].[OrdenDetalle_Entrega]  WITH CHECK ADD FOREIGN KEY([idOrden])
REFERENCES [dbo].[Orden] ([idOrden])
GO
ALTER TABLE [dbo].[OrdenDetalle_Entrega]  WITH CHECK ADD FOREIGN KEY([mecanico_responsable])
REFERENCES [dbo].[Empleado] ([idEmpleado])
GO
ALTER TABLE [dbo].[OrdenDetalle_Pack]  WITH CHECK ADD FOREIGN KEY([idOrden])
REFERENCES [dbo].[Orden] ([idOrden])
GO
ALTER TABLE [dbo].[OrdenDetalle_Pack]  WITH CHECK ADD FOREIGN KEY([idPack])
REFERENCES [dbo].[PackReparacion] ([idPack])
GO
ALTER TABLE [dbo].[OrdenDetalle_Producto]  WITH CHECK ADD FOREIGN KEY([idOrden])
REFERENCES [dbo].[Orden] ([idOrden])
GO
ALTER TABLE [dbo].[OrdenDetalle_Producto]  WITH CHECK ADD FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto_Repuesto] ([idProducto])
GO
ALTER TABLE [dbo].[OrdenDetalle_Servicio]  WITH CHECK ADD FOREIGN KEY([idOrden])
REFERENCES [dbo].[Orden] ([idOrden])
GO
ALTER TABLE [dbo].[OrdenDetalle_Servicio]  WITH CHECK ADD FOREIGN KEY([idServicio])
REFERENCES [dbo].[Servicio] ([idServicio])
GO
ALTER TABLE [dbo].[Presupuesto]  WITH CHECK ADD FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[Presupuesto]  WITH CHECK ADD FOREIGN KEY([idVehiculo])
REFERENCES [dbo].[Vehiculo] ([idVehiculo])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Pack]  WITH CHECK ADD FOREIGN KEY([idPack])
REFERENCES [dbo].[PackReparacion] ([idPack])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Pack]  WITH CHECK ADD FOREIGN KEY([idPresupuesto])
REFERENCES [dbo].[Presupuesto] ([idPresupuesto])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Producto]  WITH CHECK ADD FOREIGN KEY([idPresupuesto])
REFERENCES [dbo].[Presupuesto] ([idPresupuesto])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Producto]  WITH CHECK ADD FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto_Repuesto] ([idProducto])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Servicio]  WITH CHECK ADD FOREIGN KEY([idPresupuesto])
REFERENCES [dbo].[Presupuesto] ([idPresupuesto])
GO
ALTER TABLE [dbo].[PresupuestoDetalle_Servicio]  WITH CHECK ADD FOREIGN KEY([idServicio])
REFERENCES [dbo].[Servicio] ([idServicio])
GO
ALTER TABLE [dbo].[ProveedorProducto]  WITH CHECK ADD  CONSTRAINT [FK_ProveedorProducto_Producto] FOREIGN KEY([idProducto])
REFERENCES [dbo].[Producto_Repuesto] ([idProducto])
GO
ALTER TABLE [dbo].[ProveedorProducto] CHECK CONSTRAINT [FK_ProveedorProducto_Producto]
GO
ALTER TABLE [dbo].[ProveedorProducto]  WITH CHECK ADD  CONSTRAINT [FK_ProveedorProducto_Proveedor] FOREIGN KEY([idProveedor])
REFERENCES [dbo].[Proveedor] ([idProveedor])
GO
ALTER TABLE [dbo].[ProveedorProducto] CHECK CONSTRAINT [FK_ProveedorProducto_Proveedor]
GO
ALTER TABLE [dbo].[Vehiculo]  WITH CHECK ADD FOREIGN KEY([idCliente])
REFERENCES [dbo].[Cliente] ([idCliente])
GO
ALTER TABLE [dbo].[Orden]  WITH CHECK ADD  CONSTRAINT [CHK_Orden_Estado] CHECK  (([orEstado]='Cancelado' OR [orEstado]='Entregado' OR [orEstado]='Finalizado' OR [orEstado]='En proceso' OR [orEstado]='Pendiente'))
GO
ALTER TABLE [dbo].[Orden] CHECK CONSTRAINT [CHK_Orden_Estado]
GO
ALTER TABLE [dbo].[OrdenDetalle_Entrega]  WITH CHECK ADD  CONSTRAINT [chk_telefono_recibe_NIC] CHECK  ((len([telefono_recibe])=(8) AND [telefono_recibe] like '[2578][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'))
GO
ALTER TABLE [dbo].[OrdenDetalle_Entrega] CHECK CONSTRAINT [chk_telefono_recibe_NIC]
GO
ALTER TABLE [dbo].[Persona]  WITH CHECK ADD  CONSTRAINT [chk_Persona_Cedula_NIC] CHECK  ((len([pCedula])=(16) AND [pCedula] like '[0-9][0-9][0-9]-[0-3][0-9][0-1][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9][A-Z]'))
GO
ALTER TABLE [dbo].[Persona] CHECK CONSTRAINT [chk_Persona_Cedula_NIC]
GO
ALTER TABLE [dbo].[Persona]  WITH CHECK ADD  CONSTRAINT [chk_Persona_Telefono_NIC] CHECK  ((len([pTelefono])=(8) AND [pTelefono] like '[2578][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'))
GO
ALTER TABLE [dbo].[Persona] CHECK CONSTRAINT [chk_Persona_Telefono_NIC]
GO
/****** Object:  StoredProcedure [dbo].[AbrirCaja]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AbrirCaja]
AS
BEGIN
    SET NOCOUNT ON;

    -- 1) Si hay una caja abierta de otro día, cerrarla
    IF EXISTS (
        SELECT 1 FROM CajaDiaria WHERE cjCierre IS NULL AND cjFecha < CAST(GETDATE() AS DATE)
    )
    BEGIN
        UPDATE CajaDiaria
        SET cjCierre = '23:59:59'
        WHERE cjCierre IS NULL AND cjFecha < CAST(GETDATE() AS DATE);
    END

    -- 2) Si ya existe caja para hoy, no abrir otra
    IF NOT EXISTS (
        SELECT 1 FROM CajaDiaria WHERE cjFecha = CAST(GETDATE() AS DATE)
    )
    BEGIN
        INSERT INTO CajaDiaria (cjFecha, cjApertura, cjCierre)
        VALUES (CAST(GETDATE() AS DATE), CONVERT(TIME, GETDATE()), NULL);
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[CerrarCaja]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CerrarCaja]
AS
BEGIN
  UPDATE CajaDiaria
  SET cjCierre = CONVERT(TIME, GETDATE())
  WHERE idCaja = (
    SELECT TOP 1 idCaja
    FROM CajaDiaria
    WHERE cjCierre IS NULL
    ORDER BY idCaja DESC
  );
END
GO
/****** Object:  StoredProcedure [dbo].[Dec_StockProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Dec_StockProducto]
(
    @idProducto INT,
    @cantidad   INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    -- 1) Validar existencia del producto y que esté activo
    IF NOT EXISTS (
        SELECT 1
          FROM dbo.Producto_Repuesto
         WHERE idProducto = @idProducto
           AND prActivo = 1
    )
    BEGIN
        RAISERROR('Producto %d no existe o está inactivo.', 16, 1, @idProducto);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2) Obtener stock actual
    DECLARE @stockActual INT;
    SELECT @stockActual = prStock
      FROM dbo.Producto_Repuesto
     WHERE idProducto = @idProducto;

    -- 3) Validar que haya suficiente stock
    IF @stockActual IS NULL
    BEGIN
        RAISERROR('Error al leer el stock del producto %d.', 16, 1, @idProducto);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @cantidad <= 0
    BEGIN
        RAISERROR('La cantidad a decrementar debe ser mayor que cero.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF @cantidad > @stockActual
    BEGIN
        RAISERROR(
            'No hay suficiente stock para el producto %d. Stock actual=%d, se pidió=%d.',
            16, 1, @idProducto, @stockActual, @cantidad
        );
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 4) Actualizar stock
    UPDATE dbo.Producto_Repuesto
       SET prStock = prStock - @cantidad
     WHERE idProducto = @idProducto;

    COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Del_CompraProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 2.2 Eliminar Detalle de Compra
-- Borra un producto del detalle de una compra (basado en PK compuesta).
-------------------------------
CREATE   PROCEDURE [dbo].[Del_CompraProducto]
    @idCompra   INT,
    @idProducto INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM Compra_Producto
             WHERE idCompra   = @idCompra
               AND idProducto = @idProducto
        )
        BEGIN
            RAISERROR(
                'No existe detalle para idCompra = %d e idProducto = %d.', 
                16, 1, @idCompra, @idProducto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro de detalle
        DELETE FROM Compra_Producto
         WHERE idCompra   = @idCompra
           AND idProducto = @idProducto;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_CompraProveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 4) (Opcional) Procedure para soft-delete manual de una compra
CREATE   PROCEDURE [dbo].[Del_CompraProveedor]
    @idCompra INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Compra_Proveedores WHERE idCompra=@idCompra AND cprActivo=1)
    BEGIN
        RAISERROR('Compra %d no existe o ya está inactiva.',16,1,@idCompra);
        ROLLBACK; RETURN;
    END

    -- Usamos el trigger INSTEAD OF DELETE para hacer el soft-delete
    DELETE FROM dbo.Compra_Proveedores WHERE idCompra=@idCompra;

    COMMIT;
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_Descuento]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Del_Descuento
CREATE   PROCEDURE [dbo].[Del_Descuento]
    @IdDescuento INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento)
        BEGIN
            RAISERROR('Descuento %d no existe.', 16, 1, @IdDescuento);
            ROLLBACK TRANSACTION; RETURN;
        END

        DELETE FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_DetallePackProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------
CREATE   PROCEDURE [dbo].[Del_DetallePackProducto]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM DetallePack_Producto
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('DetallePack_Producto %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro
        DELETE FROM DetallePack_Producto
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_DetallePackServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------
CREATE   PROCEDURE [dbo].[Del_DetallePackServicio]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM DetallePack_Servicio
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('DetallePack_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro
        DELETE FROM DetallePack_Servicio
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_DetallePagoOrden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 6.3 Eliminar DetallePagoOrden
-------------------------------
CREATE   PROCEDURE [dbo].[Del_DetallePagoOrden]
    @idPagoOrden INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM DetallePagoOrden
             WHERE idPagoOrden = @idPagoOrden
        )
        BEGIN
            RAISERROR('DetallePagoOrden %d no existe.', 16, 1, @idPagoOrden);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM DetallePagoOrden
         WHERE idPagoOrden = @idPagoOrden;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_Orden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 1.3 Eliminar Orden (y sus detalles)
-------------------------------
CREATE   PROCEDURE [dbo].[Del_Orden]
    @idOrden INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia de la Orden
        IF NOT EXISTS (
            SELECT 1
              FROM Orden
             WHERE idOrden = @idOrden
        )
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar detalles de entrega
        DELETE FROM OrdenDetalle_Entrega
         WHERE idOrden = @idOrden;

        -- Eliminar detalles de servicio
        DELETE FROM OrdenDetalle_Servicio
         WHERE idOrden = @idOrden;

        -- Eliminar detalles de producto
        DELETE FROM OrdenDetalle_Producto
         WHERE idOrden = @idOrden;

        -- Eliminar detalles de pack
        DELETE FROM OrdenDetalle_Pack
         WHERE idOrden = @idOrden;

        -- Eliminar pagos asociados
        DELETE FROM DetallePagoOrden
         WHERE idOrden = @idOrden;

        -- Finalmente, eliminar la cabecera
        DELETE FROM Orden
         WHERE idOrden = @idOrden;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_OrdenDetEntrega]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 2.3 Eliminar DetalleEntrega
-------------------------------
CREATE   PROCEDURE [dbo].[Del_OrdenDetEntrega]
    @idEntrega INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM OrdenDetalle_Entrega
             WHERE idEntrega = @idEntrega
        )
        BEGIN
            RAISERROR('DetalleEntrega %d no existe.', 16, 1, @idEntrega);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM OrdenDetalle_Entrega
         WHERE idEntrega = @idEntrega;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_OrdenDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 5.3 Eliminar DetallePack
-------------------------------
CREATE   PROCEDURE [dbo].[Del_OrdenDetPack]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM OrdenDetalle_Pack
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('OrdenDetalle_Pack %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM OrdenDetalle_Pack
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_OrdenDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 4.3 Eliminar DetalleProducto
-------------------------------
CREATE   PROCEDURE [dbo].[Del_OrdenDetProducto]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM OrdenDetalle_Producto
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('OrdenDetalle_Producto %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM OrdenDetalle_Producto
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_OrdenDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 3.3 Eliminar DetalleServicio
-------------------------------
CREATE   PROCEDURE [dbo].[Del_OrdenDetServicio]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM OrdenDetalle_Servicio
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('OrdenDetalle_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DELETE FROM OrdenDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_PresupuestoDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 4.3 Eliminar DetallePresupuesto_Pack
-------------------------------
CREATE   PROCEDURE [dbo].[Del_PresupuestoDetPack]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Pack
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('PresupuestoDetalle_Pack %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro
        DELETE FROM PresupuestoDetalle_Pack
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_PresupuestoDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 3.3 Eliminar DetallePresupuesto_Producto
-------------------------------
CREATE   PROCEDURE [dbo].[Del_PresupuestoDetProducto]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Producto
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('PresupuestoDetalle_Producto %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro
        DELETE FROM PresupuestoDetalle_Producto
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_PresupuestoDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 2.3 Eliminar DetallePresupuesto_Servicio
-------------------------------
CREATE   PROCEDURE [dbo].[Del_PresupuestoDetServicio]
    @idDetalle INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Servicio
             WHERE idDetalle = @idDetalle
        )
        BEGIN
            RAISERROR('PresupuestoDetalle_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Eliminar el registro
        DELETE FROM PresupuestoDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Del_ProveedorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Del_ProveedorProducto]
  @idProveedor INT,
  @idProducto  INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;
    -- Validar que exista la relación activa
    IF NOT EXISTS (
      SELECT 1 
        FROM dbo.ProveedorProducto 
       WHERE idProveedor = @idProveedor 
         AND idProducto  = @idProducto 
         AND activo      = 1
    )
    BEGIN
      RAISERROR('No existe relación activa para desactivar.',16,1);
      ROLLBACK; RETURN;
    END

    UPDATE dbo.ProveedorProducto
       SET activo = 0
     WHERE idProveedor = @idProveedor
       AND idProducto  = @idProducto;

    COMMIT;
  END TRY
  BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
  END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Devolver_CompraProveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Devolver_CompraProveedor]
  @idCompra INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;

  DECLARE @idCajaActual INT, @compTotal FLOAT;

  ---------------------------------------------
  -- 1) Validar existencia y activo de la compra
  ---------------------------------------------
  IF NOT EXISTS (
      SELECT 1
        FROM dbo.Compra_Proveedores
       WHERE idCompra = @idCompra
         AND cprActivo = 1        -- asumo que agregaste este campo BIT
  )
  BEGIN
    RAISERROR('Compra %d no existe o ya está devuelta.', 16,1,@idCompra);
    ROLLBACK;
    RETURN;
  END

  ---------------------------------------------
  -- 2) Validar caja abierta
  ---------------------------------------------
  SELECT TOP 1 @idCajaActual = idCaja
    FROM dbo.CajaDiaria
   WHERE cjCierre IS NULL
   ORDER BY idCaja DESC;

  IF @idCajaActual IS NULL
  BEGIN
    RAISERROR('No hay caja abierta; no se puede procesar la devolución.', 16,1);
    ROLLBACK;
    RETURN;
  END

  ---------------------------------------------
  -- 3) Registrar movimiento financiero negativo
  ---------------------------------------------
  SELECT @compTotal = compTotal
    FROM dbo.Compra_Proveedores
   WHERE idCompra = @idCompra;

  INSERT INTO dbo.MovimientoFinanciero
    (movFecha, movMonto, mvTipoMovimiento, idCaja, idPagoOrden, idCompra)
  VALUES
    (GETDATE(), -@compTotal, 'DevolucionProveedor', @idCajaActual, NULL, @idCompra);

  ---------------------------------------------
  -- 4) Ajustar stock de cada producto de la compra
  ---------------------------------------------
  UPDATE pr
    SET pr.prStock = pr.prStock - cp.cproCantidad
  FROM dbo.Producto_Repuesto AS pr
  INNER JOIN dbo.Compra_Producto AS cp
    ON pr.idProducto = cp.idProducto
   AND cp.idCompra   = @idCompra
   AND cp.cproActivo  = 1;     -- asumo que también soft‐borras detalles

  ---------------------------------------------
  -- 5) “Soft‐delete” de detalles y cabecera
  ---------------------------------------------
  UPDATE p
    SET p.cprActivo = 0
  FROM dbo.Compra_Proveedores AS p
  WHERE p.idCompra = @idCompra;

  UPDATE cp
    SET cp.cproActivo = 0
  FROM dbo.Compra_Producto AS cp
  WHERE cp.idCompra = @idCompra;

  COMMIT;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_CajaDiariaDetalleFinanzas]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_CajaDiariaDetalleFinanzas]
    @Fecha DATE,
    @IdCaja INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
      m.movFecha      AS FechaHora,
      m.mvTipoMovimiento,
      COALESCE(cPersona.pNombre + ' ' + cPersona.pApellido, 
               prov.pNombre, 
               empPersona.pNombre + ' ' + empPersona.pApellido
              ) AS Responsable,
      m.movMonto      AS Monto
    FROM dbo.MovimientoFinanciero AS m
    LEFT JOIN dbo.DetallePagoOrden AS po   ON m.idPagoOrden = po.idPagoOrden
    LEFT JOIN dbo.Orden AS o                ON po.idOrden = o.idOrden
    LEFT JOIN dbo.Cliente AS c              ON po.idCliente = c.idCliente
    LEFT JOIN dbo.Persona AS cPersona       ON c.idPersona = cPersona.idPersona
    LEFT JOIN dbo.Compra_Proveedores AS cp  ON m.idCompra = cp.idCompra
    LEFT JOIN dbo.Proveedor AS prov         ON cp.idCompra = prov.idProveedor
    LEFT JOIN dbo.Empleado AS emp           ON m.idCaja IS NOT NULL AND m.idPagoOrden IS NULL AND m.idCompra IS NULL
    LEFT JOIN dbo.Persona AS empPersona     ON emp.idPersona = empPersona.idPersona
    WHERE CAST(m.movFecha AS DATE) = @Fecha
      AND (@IdCaja IS NULL OR m.idCaja = @IdCaja)
    ORDER BY m.movFecha;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_CajaDiariaIngresosEgresosPorDia]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_CajaDiariaIngresosEgresosPorDia]
    @IdCaja INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Obtener fechas inicio y fin de la caja
    DECLARE @Desde DATETIME;
    DECLARE @Hasta DATETIME;

    SELECT 
        @Desde = CjFecha,
        @Hasta = ISNULL(CjCierre, GETDATE())  -- Mantener datetime completo
    FROM dbo.CajaDiaria
    WHERE idCaja = @IdCaja;

    -- Validar que la caja exista
    IF @Desde IS NULL
    BEGIN
        RAISERROR('Caja con id %d no encontrada.', 16, 1, @IdCaja);
        RETURN;
    END

    ;WITH FechasCaja AS
    (
        SELECT CAST(@Desde AS DATE) AS Fecha  -- Convertir a DATE para solo fechas sin horas
        UNION ALL
        SELECT DATEADD(DAY, 1, Fecha)
        FROM FechasCaja
        WHERE Fecha < CAST(@Hasta AS DATE)
    )

    SELECT
      f.Fecha,
      ISNULL(SUM(CASE WHEN m.movMonto > 0 THEN m.movMonto ELSE 0 END), 0)  AS TotalIngresos,
      ISNULL(SUM(CASE WHEN m.movMonto < 0 THEN -m.movMonto ELSE 0 END), 0) AS TotalEgresos,
      ISNULL(SUM(m.movMonto), 0)                                          AS Balance
    FROM FechasCaja f
    LEFT JOIN dbo.MovimientoFinanciero m
        ON CAST(m.movFecha AS DATE) = f.Fecha
       AND m.idCaja = @IdCaja
    GROUP BY f.Fecha
    ORDER BY f.Fecha
    OPTION (MAXRECURSION 0);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_CajaDiariaResumen]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_CajaDiariaResumen]
    @Anio INT = NULL,
    @Mes  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Anio IS NULL OR @Mes IS NULL
    BEGIN
        SET @Anio = YEAR(GETDATE());
        SET @Mes  = MONTH(GETDATE());
    END

    SELECT
        c.idCaja,                                       -- ← lo agregamos
        c.CjFecha AS Fecha,
        ISNULL(SUM(CASE WHEN m.movMonto > 0 THEN m.movMonto ELSE 0 END), 0)  AS Ingreso,
        ISNULL(SUM(CASE WHEN m.movMonto < 0 THEN -m.movMonto ELSE 0 END), 0) AS Egreso,
        ISNULL(SUM(m.movMonto), 0)                                         AS Balance
    FROM CajaDiaria AS c
    LEFT JOIN MovimientoFinanciero AS m
        ON CAST(m.movFecha AS DATE) = CAST(c.CjFecha AS DATE)
    WHERE YEAR(c.CjFecha) = @Anio
      AND MONTH(c.CjFecha) = @Mes
    GROUP BY
        c.idCaja,                                       -- ← y lo agrupamos
        c.CjFecha
    ORDER BY c.CjFecha;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Clientes]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[Get_Clientes]
  @Nombre    VARCHAR(255)=NULL,
  @Activo    BIT        =NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    c.idCliente,
    p.pNombre,
    p.pApellido,
    p.pTelefono,
    COUNT(o.idOrden)     AS OrdenesRealizadas,
    c.clActivo
  FROM Cliente c
  JOIN Persona p ON p.idPersona = c.idPersona
  LEFT JOIN Orden o ON o.idCliente = c.idCliente
  WHERE (@Nombre IS NULL OR p.pNombre LIKE '%'+@Nombre+'%')
    AND (@Activo IS NULL OR c.clActivo = @Activo)
  GROUP BY
    c.idCliente,
    p.pNombre,
    p.pApellido,
    p.pTelefono,
    c.clActivo;  -- <--- aquí lo agregas
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_ClientesPorVehiculo]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Get_ClientesPorVehiculo]
  @Matricula VARCHAR(60)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    c.idCliente,
    p.pNombre, p.pApellido, p.pTelefono
  FROM Cliente c
  JOIN Persona p    ON p.idPersona = c.idPersona
  JOIN Vehiculo v   ON v.idCliente  = c.idCliente
  WHERE v.vhMatricula = @Matricula;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_CompraProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Retorna todos los productos de una compra, con información de producto.
-------------------------------
CREATE   PROCEDURE [dbo].[Get_CompraProducto]
    @idCompra INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar existencia de la compra
    IF NOT EXISTS (
        SELECT 1
          FROM Compra_Proveedores
         WHERE idCompra = @idCompra
    )
    BEGIN
        RAISERROR('Compra %d no existe.', 16, 1, @idCompra);
        RETURN;
    END

    -- Seleccionar todos los detalles de producto para esa compra
    SELECT 
        cpr.idCompra,
        cpr.idProducto,
        pr.prNombre,
        pr.prMarca,
        pr.prCategoria,
        cpr.cproCantidad,
        cpr.cproPrecioU
    FROM Compra_Producto AS cpr
    INNER JOIN Producto_Repuesto AS pr
        ON cpr.idProducto = pr.idProducto
    WHERE cpr.idCompra = @idCompra;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_CompraProv]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_CompraProv]
    @idCompra INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar existencia de la compra
    IF NOT EXISTS (
        SELECT 1
          FROM Compra_Proveedores
         WHERE idCompra = @idCompra
    )
    BEGIN
        RAISERROR('Compra %d no existe.', 16, 1, @idCompra);
        RETURN;
    END

    -- Seleccionar cabecera junto con cada producto de detalle
    SELECT 
        cp.idCompra,
        cp.compFecha,
        cp.compTotal,
        cpr.idProducto,
        pr.prNombre,
        cpr.cproCantidad,
        cpr.cproPrecioU
    FROM Compra_Proveedores AS cp
    INNER JOIN Compra_Producto AS cpr
        ON cp.idCompra = cpr.idCompra
    INNER JOIN Producto_Repuesto AS pr
        ON cpr.idProducto = pr.idProducto
    WHERE cp.idCompra = @idCompra;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Descuentos]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*******************************************
 * PROCEDIMIENTOS PARA DESCUENTOS
 * Tabla: Descuentos (IdDescuento, ID_PROD, Tipo, Valor, FechaInicio, FechaFin, Activo)
 *******************************************/

-- GET_Descuentos: opcional filtros producto y activo
CREATE   PROCEDURE [dbo].[Get_Descuentos]
  @IdProd     INT         = NULL,
  @Activo     BIT         = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    d.IdDescuento,
    d.ID_PROD,
    p.NOM_PROD AS NombreProducto,
    d.Tipo,
    d.Valor,
    d.FechaInicio,
    d.FechaFin,
    d.Activo
  FROM dbo.Descuentos d
  LEFT JOIN dbo.Productos p ON p.ID_PROD = d.ID_PROD
  WHERE (@IdProd IS NULL OR d.ID_PROD = @IdProd)
    AND (@Activo IS NULL OR d.Activo = @Activo)
  ORDER BY d.FechaInicio DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_DetallePack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_DetallePack]
  @idPack INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    'S' AS Tipo, ds.idDetalle, s.serNombre, ds.psCantidad, ds.psPrecioUnit
  FROM DetallePack_Servicio ds
  JOIN Servicio s ON s.idServicio = ds.idServicio
  WHERE ds.idPack = @idPack

  UNION ALL

  SELECT
    'P' AS Tipo, dp.idDetalle, pr.prNombre, dp.pdCantidad, dp.pdPrecioUnit
  FROM DetallePack_Producto dp
  JOIN Producto_Repuesto pr ON pr.idProducto = dp.idProducto
  WHERE dp.idPack = @idPack;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_HistorialProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_HistorialProducto]
  @idProducto INT
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @StockInicial INT;

  SELECT @StockInicial = prStock
  FROM Producto_Repuesto
  WHERE idProducto = @idProducto;

  ;WITH MovimientosProducto AS (
    -- 1) Compras al proveedor (positivas)
    SELECT
      mf.movFecha,
      mf.idMovimiento,
      'CompraProveedor' AS Tipo,
      cp.cproCantidad AS Modificacion
    FROM MovimientoFinanciero mf
    JOIN Compra_Proveedores cpr ON cpr.idCompra = mf.idCompra
    JOIN Compra_Producto cp ON cp.idCompra = cpr.idCompra
    WHERE cp.idProducto = @idProducto AND cp.cproActivo = 1
      AND mf.mvTipoMovimiento = 'CompraProveedor'

    UNION ALL

    -- 2) Devoluciones al proveedor (negativas)
    SELECT
      mf.movFecha,
      mf.idMovimiento,
      'DevolucionProveedor' AS Tipo,
      -cp.cproCantidad AS Modificacion
    FROM MovimientoFinanciero mf
    JOIN Compra_Proveedores cpr ON cpr.idCompra = mf.idCompra
    JOIN Compra_Producto cp ON cp.idCompra = cpr.idCompra
    WHERE cp.idProducto = @idProducto AND cp.cproActivo = 0
      AND mf.mvTipoMovimiento = 'DevolucionProveedor'

    UNION ALL

    -- 3) Ventas por orden (negativas)
    SELECT
      o.orFechaCreacion AS movFecha,
      0 AS idMovimiento,  -- puedes usar ID propio si tienes
      'VentaProducto' AS Tipo,
      -odp.opCantidad AS Modificacion
    FROM OrdenDetalle_Producto odp
    JOIN Orden o ON o.idOrden = odp.idOrden
    WHERE odp.idProducto = @idProducto
  )

  SELECT
    movFecha AS FechaHora,
    Tipo,
    Modificacion,
    @StockInicial + SUM(Modificacion) OVER (ORDER BY movFecha, idMovimiento) AS StockResultante
  FROM MovimientosProducto
  ORDER BY movFecha, idMovimiento;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_MovimientosFinancieros]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Get_MovimientosFinancieros]
    @Anio            INT           = NULL,
    @Mes             INT           = NULL,
    @Dia             INT           = NULL,
    @TipoMovimiento  VARCHAR(50)   = NULL,
    @PageNumber      INT           = 1,
    @PageSize        INT           = 20
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @Desde DATETIME,
        @Hasta DATETIME;

    ----------------------------------------------------------
    -- 1) Calcular rango de fechas según año/mes/día
    ----------------------------------------------------------
    IF @Dia IS NOT NULL
    BEGIN
        SET @Desde = DATEFROMPARTS(@Anio, @Mes, @Dia);
        SET @Hasta = DATEADD(SECOND, 86399, @Desde);
    END
    ELSE IF @Mes IS NOT NULL AND @Anio IS NOT NULL
    BEGIN
        SET @Desde = DATEFROMPARTS(@Anio, @Mes, 1);
        SET @Hasta = DATEADD(SECOND, -1, DATEADD(MONTH, 1, @Desde));
    END
    ELSE IF @Anio IS NOT NULL
    BEGIN
        SET @Desde = DATEFROMPARTS(@Anio, COALESCE(@Mes,1), COALESCE(@Dia,1));
        IF @Mes IS NULL
            SET @Hasta = DATEADD(SECOND, -1, DATEADD(YEAR, 1, @Desde));
        ELSE IF @Dia IS NULL
            SET @Hasta = DATEADD(SECOND, -1, DATEADD(MONTH, 1, @Desde));
    END
    ELSE
    BEGIN
        DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
        SET @Desde = DATEFROMPARTS(YEAR(@Hoy), MONTH(@Hoy), 1);
        SET @Hasta = DATEADD(SECOND, -1, DATEADD(MONTH, 1, @Desde));
    END

    ----------------------------------------------------------
    -- 2) Consulta principal con paginación
    ----------------------------------------------------------
    SELECT
        mf.idMovimiento,
        mf.movFecha,
        CONVERT(VARCHAR(10), mf.movFecha, 103)  AS Fecha,        -- DD/MM/YYYY
        CONVERT(VARCHAR(8),  mf.movFecha, 108) AS Hora,          -- HH:MM:SS
        mf.mvTipoMovimiento,
        -- Responsable: si viene de un pago de orden, tomamos al empleado; si no, 'Sistema'
        COALESCE(
          per.pNombre + ' ' + per.pApellido,
          emp.empUsuario,
          'Sistema'
        ) AS Responsable,
        dpo.metodoPago,
        mf.movMonto
    FROM dbo.MovimientoFinanciero AS mf
    -- Unimos al pago de orden para acceder al empleado
    LEFT JOIN dbo.DetallePagoOrden AS dpo
      ON mf.idPagoOrden = dpo.idPagoOrden
    LEFT JOIN dbo.Empleado AS emp
      ON dpo.idEmpleado = emp.idEmpleado
    LEFT JOIN dbo.Persona AS per
      ON emp.idPersona = per.idPersona
    WHERE mf.movFecha BETWEEN @Desde AND @Hasta
      AND (@TipoMovimiento IS NULL OR mf.mvTipoMovimiento = @TipoMovimiento)
    ORDER BY mf.movFecha DESC, mf.idMovimiento DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    ----------------------------------------------------------
    -- 3) Total de registros para paginación
    ----------------------------------------------------------
    SELECT COUNT(*) AS TotalRegistros
    FROM dbo.MovimientoFinanciero AS mf
    WHERE mf.movFecha BETWEEN @Desde AND @Hasta
      AND (@TipoMovimiento IS NULL OR mf.mvTipoMovimiento = @TipoMovimiento);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Ordenes]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Ordenes]
  @FechaDesde DATETIME = NULL,
  @FechaHasta DATETIME = NULL,
  @ClienteID  INT      = NULL,
  @Estado     VARCHAR(30)=NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    o.idOrden,
    o.orFechaCreacion,
    CONCAT(p.pNombre,' ',p.pApellido) AS Cliente,
    v.vhMarca+' / '+v.vhModelo       AS Vehículo,
    o.orFechaEntrega,
    o.orCostoTotal,
    CASE WHEN EXISTS(SELECT 1 FROM DetallePagoOrden dp WHERE dp.idOrden = o.idOrden) THEN 1 ELSE 0 END AS Pagada,
    o.orEstado
  FROM Orden o
  LEFT JOIN Cliente c ON c.idCliente = o.idCliente
  LEFT JOIN Persona p ON p.idPersona = c.idPersona
  LEFT JOIN Vehiculo v ON v.idVehiculo = o.idVehiculo
  WHERE (@FechaDesde IS NULL OR o.orFechaCreacion >= @FechaDesde)
    AND (@FechaHasta IS NULL OR o.orFechaCreacion <  DATEADD(DAY,1,@FechaHasta))
    AND (@ClienteID  IS NULL OR o.idCliente = @ClienteID)
    AND (@Estado     IS NULL OR o.orEstado  = @Estado);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_OrdenesPorCliente]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Get_OrdenesPorCliente]
(
    @idCliente   INT,
    @fechaDesde  DATETIME = NULL,
    @fechaHasta  DATETIME = NULL,
    @orEstado    VARCHAR(30) = NULL  -- filtro opcional por estado
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar cliente activo
    IF NOT EXISTS (
        SELECT 1
          FROM dbo.Cliente
         WHERE idCliente = @idCliente
           AND clActivo = 1
    )
    BEGIN
        RAISERROR('Cliente %d no existe o está inactivo.', 16, 1, @idCliente);
        RETURN;
    END

    SELECT
        o.idOrden,
        o.orFechaCreacion,
        o.orEstado,
        o.orTipo,
        o.orFechaEntrega,
        o.orFechaFin,
        o.orCostoTotal,
        v.vhMatricula      AS VehiculoMatricula,
        v.vhMarca          AS VehiculoMarca
    FROM dbo.Orden AS o
    INNER JOIN dbo.Vehiculo AS v
        ON o.idVehiculo = v.idVehiculo
    WHERE o.idCliente = @idCliente
      AND (@fechaDesde IS NULL OR o.orFechaCreacion >= @fechaDesde)
      AND (@fechaHasta IS NULL OR o.orFechaCreacion <= @fechaHasta)
      AND (@orEstado IS NULL OR o.orEstado = @orEstado)
    ORDER BY o.orFechaCreacion DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Packs]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Packs]
  @Nombre VARCHAR(60) = NULL,
  @Activo  BIT       = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT 
    idPack, 
    pckNombre, 
    pckPrecio,
    pckActivo      
  FROM PackReparacion
  WHERE (@Nombre IS NULL OR pckNombre LIKE '%' + @Nombre + '%')
    AND (@Activo IS NULL OR pckActivo = @Activo);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Presupuestos]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Presupuestos]
  @FechaDesde DATETIME = NULL,
  @FechaHasta DATETIME = NULL,
  @ClienteID  INT      = NULL,
  @Estado     BIT      = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    pre.idPresupuesto,
    pre.preFechaCreacion,
    CONCAT(p.pNombre,' ',p.pApellido) AS NombreCliente,
    pre.preEstado,
    pre.preCostoTotal
  FROM Presupuesto pre
  JOIN Cliente c    ON c.idCliente = pre.idCliente
  JOIN Persona p    ON p.idPersona = c.idPersona
  WHERE (@FechaDesde IS NULL OR pre.preFechaCreacion >= @FechaDesde)
    AND (@FechaHasta IS NULL OR pre.preFechaCreacion <  DATEADD(DAY,1,@FechaHasta))
    AND (@ClienteID  IS NULL OR pre.idCliente = @ClienteID)
    AND (@Estado     IS NULL OR pre.preEstado = @Estado);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Productos]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*******************************************
 * Get_Productos
 * Filtros actualizados para coincidir con ProductoController en C#: 
 * @Nombre (LIKE), @Marca, @Categoria, @Activo
 *******************************************/
CREATE PROCEDURE [dbo].[Get_Productos]
  @Nombre    VARCHAR(100) = NULL,
  @Marca     VARCHAR(100) = NULL,
  @Categoria VARCHAR(100) = NULL,
  @Activo    BIT          = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    idProducto,
    prNombre,
    prMarca,
    prCategoria,
    prCosto,
    prPrecioVenta,
    prStock
  FROM dbo.Producto_Repuesto
  WHERE (@Nombre IS NULL OR prNombre LIKE '%' + @Nombre + '%')
    AND (@Marca IS NULL OR prMarca = @Marca)
    AND (@Categoria IS NULL OR prCategoria = @Categoria)
    AND (@Activo IS NULL OR prActivo = @Activo)
  ORDER BY prNombre;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_ProductosPorProveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_ProductosPorProveedor]
  @idProveedor INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    pp.idProducto,
    pr.prNombre            AS Producto,
    pp.precioRef           AS PrecioReferencia,
    pp.activo              AS RelacionActiva
  FROM ProveedorProducto pp
  JOIN Producto_Repuesto pr ON pr.idProducto = pp.idProducto
  WHERE pp.idProveedor = @idProveedor;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Proveedores]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Proveedores]
  @Nombre  VARCHAR(100)=NULL,
  @Activo  BIT        =NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    idProveedor, pNombre, pTelefono, pActivo
  FROM Proveedor
  WHERE (@Nombre IS NULL OR pNombre LIKE '%'+@Nombre+'%')
    AND (@Activo IS NULL OR pActivo = @Activo);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_ProveedoresPorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_ProveedoresPorProducto]
  @idProducto INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    pp.idProveedor,
    p.pNombre      AS Proveedor,
    pp.precioRef   AS PrecioReferencia,
    pp.activo
  FROM ProveedorProducto pp
  JOIN Proveedor p ON p.idProveedor = pp.idProveedor
  WHERE pp.idProducto = @idProducto
    AND pp.activo = 1;
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_ResumenMes]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_ResumenMes]
  @Anio INT,
  @Mes  INT
AS
BEGIN
  SET NOCOUNT ON;

  -- 1) Calcular rango de fechas para el mes
  DECLARE 
    @Desde DATETIME,
    @Hasta DATETIME;

  SET @Desde = DATEFROMPARTS(@Anio, @Mes, 1);
  SET @Hasta = DATEADD(MONTH, 1, @Desde);

  ----------------------------------------------------------------
  -- 2) Totales de ingresos, egresos y balance
  ----------------------------------------------------------------
  SELECT
      TotalIngresos = ISNULL(SUM(CASE WHEN movMonto > 0 THEN movMonto END), 0),
      TotalEgresos  = ISNULL(SUM(CASE WHEN movMonto < 0 THEN -movMonto END), 0),
      Balance       = ISNULL(SUM(movMonto), 0)
    FROM dbo.MovimientoFinanciero
   WHERE movFecha >= @Desde
     AND movFecha <  @Hasta;

  ----------------------------------------------------------------
  -- 3) Últimas 5 actividades financieras
  ----------------------------------------------------------------
  SELECT TOP 5
      movFecha = FORMAT(movFecha, 'yyyy-MM-dd HH:mm'),
      mvTipoMovimiento,
      movMonto,
      idPagoOrden,
      idCompra,
      idCaja
    FROM dbo.MovimientoFinanciero
   WHERE movFecha >= @Desde
     AND movFecha <  @Hasta
   ORDER BY movFecha DESC;

  ----------------------------------------------------------------
  -- 4) Últimas 5 órdenes pagadas y entregadas
  ----------------------------------------------------------------
  SELECT TOP 5
      o.idOrden,
      o.orFechaCreacion,
      o.orEstado,
      pago.fechaPago,
      pago.montoPagado
    FROM dbo.Orden AS o
    JOIN dbo.DetallePagoOrden AS pago
      ON o.idOrden = pago.idOrden
    WHERE pago.fechaPago >= @Desde
      AND pago.fechaPago <  @Hasta
      AND o.orEstado IN ('Finalizado','Entregado')
   ORDER BY pago.fechaPago DESC;
END;

GO
/****** Object:  StoredProcedure [dbo].[Get_Servicios]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Servicios]
  @Nombre VARCHAR(60) = NULL,
  @Activo BIT = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT 
    idServicio, 
    serNombre, 
    serPrecioUnit, 
    serActivo -- <- Este campo es necesario para tu código
  FROM Servicio
  WHERE 
    (@Nombre IS NULL OR serNombre LIKE '%' + @Nombre + '%')
    AND (@Activo IS NULL OR serActivo = @Activo);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Trabajadores]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Trabajadores]
  @Nombre    VARCHAR(255) = NULL,
  @Rol       INT          = NULL,
  @Activo    BIT          = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    e.idEmpleado,               
    p.pNombre,
    p.pApellido,
    p.pTelefono,
    r.rTipo         AS Rol,
    e.empActivo     AS Activo
  FROM Empleado e
  JOIN Persona  p ON p.idPersona = e.idPersona
  JOIN Roles    r ON r.idRol     = e.idRol
  WHERE (@Nombre IS NULL OR p.pNombre LIKE '%' + @Nombre + '%')
    AND (@Rol IS NULL OR e.idRol = @Rol)
    AND (@Activo IS NULL OR e.empActivo = @Activo);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_Vehiculos]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_Vehiculos]
  @Matricula VARCHAR(60) = NULL,
  @Marca     VARCHAR(60) = NULL,
  @Activo    BIT         = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    idVehiculo,
    vhMatricula,
    vhMarca,
    vhModelo,
    vhAño,
    vhColor,
    vhActivo
  FROM Vehiculo
  WHERE (@Matricula IS NULL OR vhMatricula LIKE '%' + @Matricula + '%')
    AND (@Marca     IS NULL OR vhMarca     LIKE '%' + @Marca + '%')
    AND (@Activo    IS NULL OR vhActivo    = @Activo);
END;
GO
/****** Object:  StoredProcedure [dbo].[Get_VehiculosPorCliente]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Get_VehiculosPorCliente]
  @idCliente INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    v.vhMatricula,
    v.vhColor,
    v.vhMarca
  FROM Vehiculo v
  WHERE v.idCliente = @idCliente;
END;
GO
/****** Object:  StoredProcedure [dbo].[Inc_StockProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Inc_StockProducto]
  @idProducto   INT,
  @cantidad     INT,
  @idProveedor  INT         = NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;

  -- 1) Crear cabecera de compra
  DECLARE @idCompra INT;
  INSERT INTO dbo.Compra_Proveedores(compFecha, compTotal)
  VALUES (GETDATE(), 0);     -- compTotal lo recalculará trigger
  SET @idCompra = SCOPE_IDENTITY();

  -- 2) Insertar detalle con precio unitario
  DECLARE @precioUnit FLOAT;
  IF @idProveedor IS NOT NULL
    SELECT @precioUnit = precioRef
      FROM dbo.ProveedorProducto
     WHERE idProveedor = @idProveedor
       AND idProducto  = @idProducto;
  ELSE
    SELECT @precioUnit = prCosto
      FROM dbo.Producto_Repuesto
     WHERE idProducto = @idProducto;

  INSERT INTO dbo.Compra_Producto(
    idCompra, idProducto, idProveedor,
    cproCantidad, cproPrecioU
  )
  VALUES(
    @idCompra, @idProducto, @idProveedor,
    @cantidad, @precioUnit
  );

  COMMIT;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Cliente]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_Cliente]
    @pNombre       VARCHAR(255),
    @pApellido     VARCHAR(255),
    @pTelefono     VARCHAR(20),
    @pCedula       VARCHAR(20),
    @clEmail       VARCHAR(150),
    @OutIdCliente  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @idPersona INT;

    -- Insertar en Persona
    INSERT INTO Persona (pNombre, pApellido, pTelefono, pCedula)
    VALUES (@pNombre, @pApellido, @pTelefono, @pCedula);

    SET @idPersona = SCOPE_IDENTITY();

    -- Insertar en Cliente
    INSERT INTO Cliente (clEmail, idPersona)
    VALUES (@clEmail, @idPersona);

    -- Obtener idCliente
    SET @OutIdCliente = SCOPE_IDENTITY();

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_CompraProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_CompraProducto]
(
    @idCompra     INT,
    @idProducto   INT,
    @idProveedor  INT         = NULL,
    @cproCantidad INT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- 1) Validar existencia de la compra
        ------------------------------------------------------------
        IF NOT EXISTS (
            SELECT 1
              FROM dbo.Compra_Proveedores
             WHERE idCompra = @idCompra
        )
        BEGIN
            RAISERROR('No existe la compra %d.', 16, 1, @idCompra);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ------------------------------------------------------------
        -- 2) Validar existencia y estado del producto
        ------------------------------------------------------------
        IF NOT EXISTS (
            SELECT 1
              FROM dbo.Producto_Repuesto
             WHERE idProducto = @idProducto
               AND prActivo = 1
        )
        BEGIN
            RAISERROR('Producto %d no existe o está inactivo.', 16, 1, @idProducto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ------------------------------------------------------------
        -- 3) Determinar el precio unitario en @cproPrecioU
        ------------------------------------------------------------
        DECLARE @precioUnit FLOAT;

        IF @idProveedor IS NOT NULL
        BEGIN
            -- 3A) Validar proveedor
            IF NOT EXISTS (
                SELECT 1
                  FROM dbo.Proveedor
                 WHERE idProveedor = @idProveedor
                   AND pActivo = 1
            )
            BEGIN
                RAISERROR('Proveedor %d no existe o está inactivo.', 16, 1, @idProveedor);
                ROLLBACK TRANSACTION;
                RETURN;
            END

            -- 3B) Buscar precioRef en ProveedorProducto
            SELECT @precioUnit = precioRef
              FROM dbo.ProveedorProducto
             WHERE idProveedor = @idProveedor
               AND idProducto  = @idProducto
               AND activo      = 1;

            IF @precioUnit IS NULL
            BEGIN
                RAISERROR(
                    'No existe relación activa en ProveedorProducto para proveedor %d y producto %d.',
                    16, 1, @idProveedor, @idProducto
                );
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END
        ELSE
        BEGIN
            -- 3C) Tomar prCosto directamente de Producto_Repuesto
            SELECT @precioUnit = prCosto
              FROM dbo.Producto_Repuesto
             WHERE idProducto = @idProducto;

            IF @precioUnit IS NULL
            BEGIN
                RAISERROR(
                    'El producto %d no tiene un prCosto válido para usar como precio unitario.',
                    16, 1, @idProducto
                );
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        ------------------------------------------------------------
        -- 4) Evitar duplicados en el mismo detalle de compra
        ------------------------------------------------------------
        IF EXISTS (
            SELECT 1
              FROM dbo.Compra_Producto
             WHERE idCompra   = @idCompra
               AND idProducto = @idProducto
        )
        BEGIN
            RAISERROR(
                'El producto %d ya está en el detalle de la compra %d.',
                16, 1, @idProducto, @idCompra
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ------------------------------------------------------------
        -- 5) Insertar detalle en Compra_Producto
        ------------------------------------------------------------
        INSERT INTO dbo.Compra_Producto (
            idCompra,
            idProducto,
            idProveedor,
            cproCantidad,
            cproPrecioU
        )
        VALUES (
            @idCompra,
            @idProducto,
            @idProveedor,
            @cproCantidad,
            @precioUnit
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_CompraProv]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE PROCEDURE [dbo].[Ins_CompraProv]
  --@compFecha DATETIME solo cambie esto
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @idCajaActual INT;
    SELECT TOP 1 @idCajaActual = idCaja
      FROM dbo.CajaDiaria
     WHERE cjCierre IS NULL
     ORDER BY idCaja DESC;

    IF @idCajaActual IS NULL
    BEGIN
        RAISERROR('No hay caja abierta; no se puede registrar la compra.',16,1);
        RETURN;    -- sin BEGIN/COMMIT, no se hará nada
    END


  BEGIN TRANSACTION;
    INSERT INTO dbo.Compra_Proveedores (compFecha, compTotal)
    VALUES (GETDATE(), 0);  -- el trigger trg_RecalculaCompTotal_After_CompraProducto ajustará compTotal
  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Descuento]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Ins_Descuento
CREATE   PROCEDURE [dbo].[Ins_Descuento]
    @ID_PROD      INT,
    @Tipo         VARCHAR(20),
    @Valor        FLOAT,
    @FechaInicio  DATETIME,
    @FechaFin     DATETIME,
    @Activo       BIT = 1,
    @OUT_IdDes    INT = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validaciones básicas
        IF @ID_PROD IS NULL
        BEGIN
            RAISERROR('ID_PROD es requerido.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @Tipo IS NULL OR LEN(LTRIM(RTRIM(@Tipo))) = 0
        BEGIN
            RAISERROR('Tipo es requerido (Porcentaje o Fijo).', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @FechaInicio IS NULL OR @FechaFin IS NULL
        BEGIN
            RAISERROR('Fechas Inicio y Fin son requeridas.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @FechaInicio >= @FechaFin
        BEGIN
            RAISERROR('FechaInicio debe ser anterior a FechaFin.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Normalizar tipo y validar valor
        DECLARE @tNorm VARCHAR(20) = UPPER(LTRIM(RTRIM(@Tipo)));
        IF @tNorm NOT IN ('PORCENTAJE','FIJO')
        BEGIN
            RAISERROR('Tipo inválido. Use ''Porcentaje'' o ''Fijo''.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @tNorm = 'PORCENTAJE'
        BEGIN
            IF @Valor < 0 OR @Valor > 100
            BEGIN
                RAISERROR('Valor inválido para Porcentaje (0-100).', 16, 1);
                ROLLBACK TRANSACTION; RETURN;
            END
        END
        ELSE
        BEGIN
            IF @Valor < 0
            BEGIN
                RAISERROR('Valor inválido para Fijo (>=0).', 16, 1);
                ROLLBACK TRANSACTION; RETURN;
            END
        END

        -- Validar existencia de producto
        IF NOT EXISTS (SELECT 1 FROM dbo.Productos WHERE ID_PROD = @ID_PROD)
        BEGIN
            RAISERROR('Producto %d no existe.', 16, 1, @ID_PROD);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Validar solapamiento con otros descuentos activos para el mismo producto
        IF EXISTS (
            SELECT 1 FROM dbo.Descuentos d
            WHERE d.ID_PROD = @ID_PROD
              AND ISNULL(d.Activo,1) = 1
              AND d.FechaInicio < @FechaFin
              AND d.FechaFin > @FechaInicio
        )
        BEGIN
            RAISERROR('Existe un descuento activo solapado para este producto en el rango de fechas.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Insert
        INSERT INTO dbo.Descuentos (ID_PROD, Tipo, Valor, FechaInicio, FechaFin, Activo)
        VALUES (@ID_PROD, CASE WHEN @tNorm='PORCENTAJE' THEN 'Porcentaje' ELSE 'Fijo' END, @Valor, @FechaInicio, @FechaFin, ISNULL(@Activo,1));

        SET @OUT_IdDes = CONVERT(INT, SCOPE_IDENTITY());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_DetallePackProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Ins_DetallePackProducto]
    @idPack       INT,
    @idProducto   INT,
    @pdCantidad   INT   = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia y estado del pack:
        IF NOT EXISTS (
            SELECT 1
              FROM PackReparacion
             WHERE idPack = @idPack
               AND pckActivo = 1
        )
        BEGIN
            RAISERROR('PackReparacion %d no existe o está inactivo.', 16, 1, @idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener el precio unitario del producto (tomamos prPrecioVenta):
        DECLARE @precioProducto FLOAT;
        SELECT @precioProducto = prPrecioVenta
          FROM Producto_Repuesto
         WHERE idProducto = @idProducto
           AND prActivo = 1;

        IF @precioProducto IS NULL
        BEGIN
            RAISERROR('Producto_Repuesto %d no existe o está inactivo.', 16, 1, @idProducto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3) Evitar duplicar la misma relación pack–producto:
        IF EXISTS (
            SELECT 1
              FROM DetallePack_Producto
             WHERE idPack     = @idPack
               AND idProducto = @idProducto
        )
        BEGIN
            RAISERROR(
                'El producto %d ya está asignado al pack %d.',
                16, 1, @idProducto, @idPack
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Insertar el detalle, copiando precio desde Producto_Repuesto:
        INSERT INTO DetallePack_Producto
            (pdCantidad, pdPrecioUnit, idPack, idProducto)
        VALUES
            (@pdCantidad, @precioProducto, @idPack, @idProducto);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_DetallePackServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Ins_DetallePackServicio]
    @idPack     INT,
    @idServicio INT,
    @psCantidad INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia y estado del pack:
        IF NOT EXISTS (
            SELECT 1
              FROM PackReparacion
             WHERE idPack = @idPack
               AND pckActivo = 1
        )
        BEGIN
            RAISERROR('PackReparacion %d no existe o está inactivo.', 16, 1, @idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener el precio unitario del servicio:
        DECLARE @precioServicio FLOAT;
        SELECT @precioServicio = serPrecioUnit
          FROM Servicio
         WHERE idServicio = @idServicio
           AND serActivo = 1;

        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServicio);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3) Evitar duplicar la misma relación pack–servicio:
        IF EXISTS (
            SELECT 1
              FROM DetallePack_Servicio
             WHERE idPack     = @idPack
               AND idServicio = @idServicio
        )
        BEGIN
            RAISERROR(
                'El servicio %d ya está asignado al pack %d.',
                16, 1, @idServicio, @idPack
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Insertar el detalle, copiando precio desde Servicio:
        INSERT INTO DetallePack_Servicio
            (psCantidad, psPrecioUnit, idPack, idServicio)
        VALUES
            (@psCantidad, @precioServicio, @idPack, @idServicio);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_DetallePagoOrden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_DetallePagoOrden]
  @idOrden       INT,
  @metodoPago    VARCHAR(50),
  @observaciones VARCHAR(255) = NULL,
  @idEmpleado    INT,
  @idCliente     INT
AS
BEGIN
  SET NOCOUNT ON;

   DECLARE @idCajaActual INT;
    SELECT TOP 1 @idCajaActual = idCaja
      FROM dbo.CajaDiaria
     WHERE cjCierre IS NULL
     ORDER BY idCaja DESC;
    IF @idCajaActual IS NULL
    BEGIN
        RAISERROR('No hay caja abierta; no se puede registrar el pago.',16,1);
        RETURN;
    END

  DECLARE @totalOrden   FLOAT,
          @pagadoHastaAhora FLOAT,
          @aPagar FLOAT;

  BEGIN TRANSACTION;
    SELECT @totalOrden   = orCostoTotal FROM dbo.Orden WHERE idOrden = @idOrden;
    SELECT @pagadoHastaAhora = ISNULL(SUM(montoPagado),0)
      FROM dbo.DetallePagoOrden WHERE idOrden = @idOrden;
    SET @aPagar = @totalOrden - @pagadoHastaAhora;

    IF @aPagar <= 0
    BEGIN RAISERROR('No hay saldo pendiente para la orden %d.',16,1,@idOrden);
          ROLLBACK; RETURN;
    END

    INSERT INTO dbo.DetallePagoOrden
      (idOrden, fechaPago, montoPagado, metodoPago, observaciones, idEmpleado, idCliente)
    VALUES
      (@idOrden, GETDATE(), @aPagar, @metodoPago, @observaciones, @idEmpleado, @idCliente);

  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Empleado]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Ins_Empleado]
    @pNombre VARCHAR(255),
    @pApellido VARCHAR(255),
    @pTelefono VARCHAR(20),
    @pCedula VARCHAR(20),
    @empUsuario VARCHAR(20),
    @idRol INT,
    @contrasena_texto VARCHAR(128) 
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @idPersona INT;

    -- Insertar en Persona
    INSERT INTO Persona (pNombre, pApellido, pTelefono, pCedula)
    VALUES (@pNombre, @pApellido, @pTelefono, @pCedula);

    -- Obtener el ID de Persona recién insertado
    SET @idPersona = SCOPE_IDENTITY();

    -- Insertar en Empleado (sin `empContrasena`, ya que el trigger lo cifrará)
    INSERT INTO Empleado (empUsuario, idRol, empActivo, idPersona, contrasena_texto)
    VALUES (@empUsuario, @idRol, 1, @idPersona, @contrasena_texto);

    -- Confirmar transacción
    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_MovimientoFinanciero]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------
-- 4. CAJA DIARIA Y MOVIMIENTOS FINANCIEROS
--------------------------------------------------------------------------------

-------------------------------
-- 4.1 Registrar Movimiento Financiero Genérico
-- Permite insertar cualquier ingreso/egreso en la caja abierta
-------------------------------
CREATE   PROCEDURE [dbo].[Ins_MovimientoFinanciero]
    @movFecha           DATETIME,
    @movMonto           FLOAT,
    @mvTipoMovimiento   VARCHAR(50),
    @idPagoOrden        INT    = NULL,
    @idCompra           INT    = NULL,
    @idEmpleado         INT    = NULL  -- opcional, quién registra
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar existencia de caja abierta
        DECLARE @idCajaActual INT;
        SELECT TOP 1 @idCajaActual = idCaja
          FROM CajaDiaria
         WHERE cjCierre IS NULL
         ORDER BY idCaja DESC;

        IF @idCajaActual IS NULL
        BEGIN
            RAISERROR('No existe una caja abierta.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar si se especifica idPagoOrden
        IF @idPagoOrden IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                  FROM DetallePagoOrden
                 WHERE idPagoOrden = @idPagoOrden
            )
            BEGIN
                RAISERROR('Pago de orden %d no existe.', 16, 1, @idPagoOrden);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- Validar si se especifica idCompra
        IF @idCompra IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                  FROM Compra_Proveedores
                 WHERE idCompra = @idCompra
            )
            BEGIN
                RAISERROR('Compra %d no existe.', 16, 1, @idCompra);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- Insertar movimiento
        INSERT INTO MovimientoFinanciero
            (movFecha, movMonto, mvTipoMovimiento, idCaja, idPagoOrden, idCompra)
        VALUES
            (@movFecha, @movMonto, @mvTipoMovimiento, @idCajaActual, @idPagoOrden, @idCompra);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Orden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 1.1 Insertar Orden
-------------------------------
CREATE PROCEDURE [dbo].[Ins_Orden]
 --   @orFechaCreacion   DATETIME,
    @orEstado          VARCHAR(30),
    --@orCostoTotal      FLOAT,
    @orTipo            VARCHAR(50),
    @orFechaEntrega    DATETIME,
    @orFechaFin        DATETIME    = NULL,
    @orFalla           VARCHAR(MAX),
    @idCliente         INT,
    @idVehiculo        INT,
    @idPresupuesto     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Validar cliente
        IF NOT EXISTS (
            SELECT 1
              FROM Cliente
             WHERE idCliente = @idCliente
               AND clActivo = 1
        )
        BEGIN
            RAISERROR('Cliente %d no existe o está inactivo.', 16, 1, @idCliente);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Validar vehículo activo
        IF NOT EXISTS (
            SELECT 1
              FROM Vehiculo
             WHERE idVehiculo = @idVehiculo
               AND vhActivo = 1
        )
        BEGIN
            RAISERROR('Vehículo %d no existe o no está disponible.', 16, 1, @idVehiculo);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Si envían @idPresupuesto, validar que exista
        IF @idPresupuesto IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                  FROM Presupuesto
                 WHERE idPresupuesto = @idPresupuesto
            )
            BEGIN
                RAISERROR('Presupuesto %d no existe.', 16, 1, @IdPresupuesto);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END
		

        -- 4. Insertar la orden (puede venir @idPresupuesto = NULL)
        INSERT INTO Orden
            (orFechaCreacion, orEstado, orCostoTotal, orTipo,
             orFechaEntrega, orFechaFin, orFalla,
             idCliente, idVehiculo, idPresupuesto)
        VALUES
            (GETDATE(), @orEstado, 0, @orTipo,
             @orFechaEntrega, @orFechaFin, @orFalla,
             @idCliente, @idVehiculo, @idPresupuesto);
			 --cambie @orCostoTotal por 0, si no funciona revertirlo y poner tambien en la parte de los atributos, tambien @orFechaCreacion por GETDATE()
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_OrdenDesdePresupuesto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_OrdenDesdePresupuesto]
    @idPresupuesto INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        ----------------------------------------------------------------
        -- 1) Verificar que el presupuesto existe y esté aprobado
        ----------------------------------------------------------------
        IF NOT EXISTS (
            SELECT 1
              FROM Presupuesto
             WHERE idPresupuesto = @idPresupuesto
               AND preEstado = 1
        )
        BEGIN
            RAISERROR('Presupuesto %d no existe o no está aprobado.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ----------------------------------------------------------------
        -- 2) Validar stock de productos directos del presupuesto
        ----------------------------------------------------------------
        IF EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Producto pd
              JOIN Producto_Repuesto pr
                ON pr.idProducto = pd.idProducto
             WHERE pd.idPresupuesto = @idPresupuesto
               AND pr.prStock < pd.ppCantidad
        )
        BEGIN
            RAISERROR(
                'No hay stock suficiente para al menos uno de los productos directos del presupuesto %d.',
                16, 1, @idPresupuesto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ----------------------------------------------------------------
        -- 3) Validar stock para cada pack del presupuesto
        ----------------------------------------------------------------
        IF EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Pack pp
              JOIN DetallePack_Producto dpp
                ON dpp.idPack = pp.idPack
              JOIN Producto_Repuesto pr
                ON pr.idProducto = dpp.idProducto
             WHERE pp.idPresupuesto = @idPresupuesto
               -- cantidad total requerida = cantidad de packs × cantidad de producto por pack
               AND pr.prStock < (pp.ppCantidad * dpp.pdCantidad)
        )
        BEGIN
            RAISERROR(
                'No hay stock suficiente para al menos uno de los packs del presupuesto %d.',
                16, 1, @idPresupuesto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        ----------------------------------------------------------------
        -- 4) Obtener datos del presupuesto
        ----------------------------------------------------------------
        DECLARE
            @preFalla    VARCHAR(MAX),
            @idCliente   INT,
            @idVehiculo  INT;

        SELECT
            @preFalla   = ISNULL(preFalla, ''),
            @idCliente  = idCliente,
            @idVehiculo = idVehiculo
        FROM Presupuesto
        WHERE idPresupuesto = @idPresupuesto;

        ----------------------------------------------------------------
        -- 5) Insertar Orden y capturar ID en variable local
        ----------------------------------------------------------------
        DECLARE @idOrden INT;

        INSERT INTO Orden
            (
              orFechaCreacion,
              orEstado,
              orCostoTotal,
              orTipo,
              orFechaEntrega,
              orFechaFin,
              orFalla,
              idCliente,
              idVehiculo,
              idPresupuesto
            )
        VALUES
            (
              GETDATE(),
              'Pendiente',
              0,
              NULL,
              NULL,
              NULL,
              @preFalla,
              @idCliente,
              @idVehiculo,
              @idPresupuesto
            );

        SET @idOrden = SCOPE_IDENTITY();

        ----------------------------------------------------------------
        -- 6) Copiar detalles desde presupuesto
        ----------------------------------------------------------------
        INSERT INTO OrdenDetalle_Servicio (idOrden, idServicio, opCantidad, opPrecioUnit)
        SELECT
            @idOrden,
            idServicio,
            psCantidad,
            psPrecioUnit
        FROM PresupuestoDetalle_Servicio
        WHERE idPresupuesto = @idPresupuesto;

        INSERT INTO OrdenDetalle_Producto (idOrden, idProducto, opCantidad, opPrecioUnit)
        SELECT
            @idOrden,
            idProducto,
            ppCantidad,
            ppPrecioUnit
        FROM PresupuestoDetalle_Producto
        WHERE idPresupuesto = @idPresupuesto;

        INSERT INTO OrdenDetalle_Pack (idOrden, idPack, opCantidad, opPrecioPack)
        SELECT
            @idOrden,
            idPack,
            ppCantidad,
            ppPrecioPack
        FROM PresupuestoDetalle_Pack
        WHERE idPresupuesto = @idPresupuesto;

        ----------------------------------------------------------------
        -- 7) Confirmar y devolver ID generado
        ----------------------------------------------------------------
        COMMIT TRANSACTION;
        SELECT @idOrden AS IdOrden;

    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_OrdenDetEntrega]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROCEDURE [dbo].[Ins_OrdenDetEntrega]
    @idOrden              INT,
    @recibido_por         VARCHAR(100),
    @telefono_recibe      VARCHAR(20),
    @es_propietario       BIT,
    @entregado_por        INT,
--    @fecha_entrega        DATETIME, quitar comentariado en caso de que no funcione
    @orden_entrega        VARCHAR(50),
    @mecanico_responsable INT,
    @informe_tecnico      VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Validar existencia de la Orden
        IF NOT EXISTS (
            SELECT 1
              FROM Orden
             WHERE idOrden = @idOrden
        )
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Validar que el empleado que entrega exista y esté activo
        IF NOT EXISTS (
            SELECT 1
              FROM Empleado
             WHERE idEmpleado = @entregado_por
               AND empActivo = 1
        )
        BEGIN
            RAISERROR('Empleado (entregado_por) %d no existe o está inactivo.', 16, 1, @entregado_por);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Validar que el mecánico responsable exista y esté activo
        IF NOT EXISTS (
            SELECT 1
              FROM Empleado
             WHERE idEmpleado = @mecanico_responsable
               AND empActivo = 1
        )
        BEGIN
            RAISERROR('Empleado (mecanico_responsable) %d no existe o está inactivo.', 16, 1, @mecanico_responsable);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. Insertar detalle de entrega
        INSERT INTO OrdenDetalle_Entrega
            (idOrden, recibido_por, telefono_recibe, es_propietario,
             entregado_por, fecha_entrega, orden_entrega,
             mecanico_responsable, informe_tecnico)
        VALUES
            (@idOrden, @recibido_por, @telefono_recibe, @es_propietario,
             @entregado_por, GETDATE(), @orden_entrega,
             @mecanico_responsable, @informe_tecnico);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_OrdenDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_OrdenDetPack]
    @idOrden    INT,
    @idPack     INT,
    @opCantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia de la orden
        IF NOT EXISTS (
            SELECT 1
              FROM dbo.Orden
             WHERE idOrden = @idOrden
        )
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- 2) Validar estado de orden
        IF EXISTS (
            SELECT 1
              FROM dbo.Orden
             WHERE idOrden  = @idOrden
               AND orEstado IN ('Finalizado','Cancelado','Entregado')
        )
        BEGIN
            DECLARE @estadoActual VARCHAR(30) = (SELECT orEstado FROM dbo.Orden WHERE idOrden=@idOrden);
            RAISERROR('No se pueden agregar packs a una orden en estado %s.',16,1,@estadoActual);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- 3) Validar pack activo
        IF NOT EXISTS (
            SELECT 1
              FROM dbo.PackReparacion
             WHERE idPack = @idPack
               AND pckActivo = 1
        )
        BEGIN
            RAISERROR('Pack %d no existe o está inactivo.',16,1,@idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- 4) Verificar stock de cada componente del pack
        DECLARE @insuficiente BIT = 0;
        SELECT TOP 1 @insuficiente = 1
          FROM dbo.DetallePack_Producto dp
          JOIN dbo.Producto_Repuesto pr
            ON pr.idProducto = dp.idProducto
         WHERE dp.idPack = @idPack
           AND pr.prActivo = 1
           AND pr.prStock < dp.pdCantidad * @opCantidad;

        IF @insuficiente = 1
        BEGIN
            RAISERROR('Stock insuficiente para alguno de los componentes del pack %d.',16,1,@idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- 5) Evitar duplicar pack en la orden
        IF EXISTS (
            SELECT 1
              FROM dbo.OrdenDetalle_Pack
             WHERE idOrden = @idOrden
               AND idPack  = @idPack
        )
        BEGIN
            RAISERROR('El pack %d ya está en la orden %d.',16,1,@idPack,@idOrden);
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        -- 6) Insertar detalle con precio actual del pack
        INSERT INTO dbo.OrdenDetalle_Pack
            (idOrden, idPack, opCantidad, opPrecioPack)
        SELECT
            @idOrden, @idPack, @opCantidad, pckPrecio
          FROM dbo.PackReparacion
         WHERE idPack = @idPack;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_OrdenDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_OrdenDetProducto]
    @idOrden    INT,
    @idProducto INT,
    @opCantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia de la orden
        IF NOT EXISTS (SELECT 1 FROM dbo.Orden WHERE idOrden = @idOrden)
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 2) Validar estado de la orden
        DECLARE @estadoOrden VARCHAR(30)
        SELECT @estadoOrden = orEstado
          FROM dbo.Orden
         WHERE idOrden = @idOrden

        IF @estadoOrden IN ('Finalizado','Entregado','Cancelado')
        BEGIN
            RAISERROR('No se puede agregar productos a una orden en estado %s.', 16, 1, @estadoOrden)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 3) Validar existencia y stock del producto
        DECLARE @stockActual INT
        SELECT @stockActual = prStock
          FROM dbo.Producto_Repuesto
         WHERE idProducto = @idProducto
           AND prActivo = 1

        IF @stockActual IS NULL
        BEGIN
            RAISERROR('Producto %d no existe o está inactivo.', 16, 1, @idProducto)
            ROLLBACK TRANSACTION
            RETURN
        END

        IF @opCantidad > @stockActual
        BEGIN
            RAISERROR('Stock insuficiente para producto %d. Disponible=%d, Solicitado=%d.', 16, 1, @idProducto, @stockActual, @opCantidad)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 4) Evitar duplicados
        IF EXISTS (
            SELECT 1 FROM dbo.OrdenDetalle_Producto
            WHERE idOrden = @idOrden AND idProducto = @idProducto
        )
        BEGIN
            RAISERROR('El producto %d ya está en la orden %d.', 16, 1, @idProducto, @idOrden)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 5) Insertar detalle
        INSERT INTO dbo.OrdenDetalle_Producto (idOrden, idProducto, opCantidad, opPrecioUnit)
        SELECT @idOrden, @idProducto, @opCantidad, prPrecioVenta
          FROM dbo.Producto_Repuesto
         WHERE idProducto = @idProducto

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_OrdenDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_OrdenDetServicio]
    @idOrden     INT,
    @idServicio  INT,
    @opCantidad  INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia de la orden
        IF NOT EXISTS (SELECT 1 FROM dbo.Orden WHERE idOrden = @idOrden)
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 2) Validar estado de la orden
        DECLARE @estadoServ VARCHAR(30)
        SELECT @estadoServ = orEstado
          FROM dbo.Orden
         WHERE idOrden = @idOrden

        IF @estadoServ IN ('Finalizado','Entregado','Cancelado')
        BEGIN
            RAISERROR('No se pueden agregar servicios a una orden en estado %s.', 16, 1, @estadoServ)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 3) Obtener precio del servicio
        DECLARE @precioServicio FLOAT
        SELECT @precioServicio = serPrecioUnit
          FROM dbo.Servicio
         WHERE idServicio = @idServicio
           AND serActivo = 1

        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServicio)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 4) Evitar duplicados
        IF EXISTS (
            SELECT 1 FROM dbo.OrdenDetalle_Servicio
            WHERE idOrden = @idOrden AND idServicio = @idServicio
        )
        BEGIN
            RAISERROR('El servicio %d ya está en la orden %d.', 16, 1, @idServicio, @idOrden)
            ROLLBACK TRANSACTION
            RETURN
        END

        -- 5) Insertar detalle
        INSERT INTO dbo.OrdenDetalle_Servicio (idOrden, idServicio, opCantidad, opPrecioUnit)
        VALUES (@idOrden, @idServicio, @opCantidad, @precioServicio)

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_PackReparacion]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_PackReparacion]
  @pckNombre      VARCHAR(60),
  @pckDescripcion VARCHAR(250) = NULL,
  @NewPackId      INT OUTPUT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;
    IF EXISTS (SELECT 1 
               FROM dbo.PackReparacion 
               WHERE pckNombre = @pckNombre 
                 AND pckActivo = 1)
    BEGIN
      RAISERROR('Ya existe pack activo %s.',16,1,@pckNombre);
      ROLLBACK; 
      RETURN;
    END

    INSERT INTO dbo.PackReparacion (pckNombre, pckDescripcion, pckActivo)
    VALUES (@pckNombre, @pckDescripcion, 1);

    -- Capturamos el nuevo ID
    SET @NewPackId = CAST(SCOPE_IDENTITY() AS INT);

  COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Ins_PagoProveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Ins_PagoProveedor]
    @idCompra       INT,
    @fechaPago      DATETIME,
    @montoPagado    FLOAT,
    @idEmpleado     INT,
    @idProveedor    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar existencia de la compra
        IF NOT EXISTS (
            SELECT 1
              FROM Compra_Proveedores
             WHERE idCompra = @idCompra
        )
        BEGIN
            RAISERROR('Compra %d no existe.', 16, 1, @idCompra);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar existencia y estado del empleado que realiza el pago
        IF NOT EXISTS (
            SELECT 1
              FROM Empleado
             WHERE idEmpleado = @idEmpleado
               AND empActivo = 1
        )
        BEGIN
            RAISERROR('Empleado %d no existe o está inactivo.', 16, 1, @idEmpleado);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar existencia del proveedor
        IF NOT EXISTS (
            SELECT 1
              FROM Proveedor
             WHERE idProveedor = @idProveedor
               AND pActivo = 1
        )
        BEGIN
            RAISERROR('Proveedor %d no existe o está inactivo.', 16, 1, @idProveedor);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Obtener la caja diaria activa (la más reciente sin cierre)
        DECLARE @idCajaActual INT;
        SELECT TOP 1 @idCajaActual = idCaja
          FROM CajaDiaria
         WHERE cjCierre IS NULL
         ORDER BY idCaja DESC;

        IF @idCajaActual IS NULL
        BEGIN
            RAISERROR('No existe una caja abierta.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insertar en MovimientoFinanciero
        INSERT INTO MovimientoFinanciero
            (movFecha, movMonto, mvTipoMovimiento, idCaja, idPagoOrden, idCompra)
        VALUES
            (@fechaPago, @montoPagado, 'PagoProveedor', @idCajaActual, NULL, @idCompra);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Presupuesto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_Presupuesto]
    @preFalla   VARCHAR(MAX),
    @idVehiculo INT = NULL,
    @idCliente  INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar existencia del Vehículo
        IF NOT EXISTS (
            SELECT 1 FROM Vehiculo WHERE idVehiculo = @idVehiculo
        )
        BEGIN
            RAISERROR('Vehículo %d no existe.', 16, 1, @idVehiculo);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar existencia y estado del Cliente
        IF NOT EXISTS (
            SELECT 1 FROM Cliente WHERE idCliente = @idCliente AND clActivo = 1
        )
        BEGIN
            RAISERROR('Cliente %d no existe o está inactivo.', 16, 1, @idCliente);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insertar presupuesto
        INSERT INTO Presupuesto
            (preFechaCreacion, preCostoTotal, preFalla, idVehiculo, idCliente)
        VALUES
            (GETDATE(), 0, @preFalla, @idVehiculo, @idCliente);

        -- RETORNAR el ID generado
        SELECT SCOPE_IDENTITY() AS IdPresupuesto;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_PresupuestoDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Ins_PresupuestoDetPack]
    @idPresupuesto INT,
    @idPack        INT,
    @ppCantidad    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia y estado del presupuesto:
        IF NOT EXISTS (
            SELECT 1
              FROM Presupuesto
             WHERE idPresupuesto = @idPresupuesto
               AND preEstado = 0    -- editable
        )
        BEGIN
            RAISERROR('Presupuesto %d no existe o no está en estado editable.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener el precio del pack:
        DECLARE @precioPack FLOAT;
        SELECT @precioPack = pckPrecio
          FROM PackReparacion
         WHERE idPack = @idPack
           AND pckActivo = 1;

        IF @precioPack IS NULL
        BEGIN
            RAISERROR('PackReparacion %d no existe o está inactivo.', 16, 1, @idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3) Evitar duplicar mismo pack en el presupuesto:
        IF EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Pack
             WHERE idPresupuesto = @idPresupuesto
               AND idPack         = @idPack
        )
        BEGIN
            RAISERROR(
                'El pack %d ya está en el presupuesto %d.',
                16, 1, @idPack, @idPresupuesto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Insertar detalle, copiando el precio del pack:
        INSERT INTO PresupuestoDetalle_Pack
            (idPresupuesto, idPack, ppCantidad, ppPrecioPack)
        VALUES
            (@idPresupuesto, @idPack, @ppCantidad, @precioPack);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_PresupuestoDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Ins_PresupuestoDetProducto]
    @idPresupuesto INT,
    @idProducto    INT,
    @ppCantidad    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia y estado del presupuesto:
        IF NOT EXISTS (
            SELECT 1
              FROM Presupuesto
             WHERE idPresupuesto = @idPresupuesto
               AND preEstado = 0    -- editable
        )
        BEGIN
            RAISERROR('Presupuesto %d no existe o no está en estado editable.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener el precio unitario del producto:
        DECLARE @precioProducto FLOAT;
        SELECT @precioProducto = prPrecioVenta
          FROM Producto_Repuesto
         WHERE idProducto = @idProducto
           AND prActivo = 1;

        IF @precioProducto IS NULL
        BEGIN
            RAISERROR('Producto %d no existe o está inactivo.', 16, 1, @idProducto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3) Evitar duplicar mismo producto en el presupuesto:
        IF EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Producto
             WHERE idPresupuesto = @idPresupuesto
               AND idProducto     = @idProducto
        )
        BEGIN
            RAISERROR(
                'El producto %d ya está en el presupuesto %d.',
                16, 1, @idProducto, @idPresupuesto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Insertar detalle, copiando el precio:
        INSERT INTO PresupuestoDetalle_Producto
            (idPresupuesto, idProducto, ppCantidad, ppPrecioUnit)
        VALUES
            (@idPresupuesto, @idProducto, @ppCantidad, @precioProducto);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_PresupuestoDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Ins_PresupuestoDetServicio]
    @idPresupuesto INT,
    @idServicio    INT,
    @psCantidad    INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Validar existencia y estado del presupuesto:
        IF NOT EXISTS (
            SELECT 1
              FROM Presupuesto
             WHERE idPresupuesto = @idPresupuesto
               AND preEstado = 0    -- editable
        )
        BEGIN
            RAISERROR('Presupuesto %d no existe o no está en estado editable.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener el precio unitario del servicio:
        DECLARE @precioServicio FLOAT;
        SELECT @precioServicio = serPrecioUnit
          FROM Servicio
         WHERE idServicio = @idServicio
           AND serActivo = 1;

        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServicio);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3) Evitar duplicar mismo servicio en el presupuesto:
        IF EXISTS (
            SELECT 1
              FROM PresupuestoDetalle_Servicio
             WHERE idPresupuesto = @idPresupuesto
               AND idServicio    = @idServicio
        )
        BEGIN
            RAISERROR(
                'El servicio %d ya está en el presupuesto %d.',
                16, 1, @idServicio, @idPresupuesto
            );
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Insertar detalle, copiando el precio:
        INSERT INTO PresupuestoDetalle_Servicio
            (idPresupuesto, idServicio, psCantidad, psPrecioUnit)
        VALUES
            (@idPresupuesto, @idServicio, @psCantidad, @precioServicio);


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Ins_Producto]
    @prNombre       VARCHAR(100),
    @prMarca        VARCHAR(100) = NULL,
    @prCategoria    VARCHAR(100) = NULL,
    @prCosto        FLOAT        = NULL,
    @prPrecioVenta  FLOAT        = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF @prNombre IS NULL OR LEN(LTRIM(RTRIM(@prNombre))) = 0
        BEGIN
            RAISERROR('El nombre del producto es requerido.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insertar en la tabla correcta
        INSERT INTO dbo.Producto_Repuesto
            (prNombre, prMarca, prCategoria, prCosto, prPrecioVenta, prStock, prActivo)
        VALUES
            (@prNombre, @prMarca, @prCategoria, @prCosto, @prPrecioVenta, 0, 1);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Proveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Ins_Proveedor]
    @pNombre   VARCHAR(100),
    @pTelefono VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
		
        IF EXISTS (
            SELECT 1
              FROM Proveedor
             WHERE pNombre = @pNombre
               AND pActivo = 1
        )
        BEGIN
            RAISERROR('Ya existe un proveedor activo con el nombre %s.', 16, 1, @pNombre);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO Proveedor (pNombre, pTelefono)
        VALUES (@pNombre, @pTelefono);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_ProveedorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Ins_ProveedorProducto]
    @idProveedor INT,
    @idProducto  INT,
    @precioRef   FLOAT,
    @activo      BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    -- Validar proveedor y producto
    IF NOT EXISTS (SELECT 1 FROM dbo.Proveedor WHERE idProveedor = @idProveedor AND pActivo = 1)
    BEGIN
        RAISERROR('Proveedor %d inválido o inactivo.',16,1,@idProveedor);
        ROLLBACK; RETURN;
    END
    IF NOT EXISTS (SELECT 1 FROM dbo.Producto_Repuesto WHERE idProducto = @idProducto AND prActivo = 1)
    BEGIN
        RAISERROR('Producto %d inválido o inactivo.',16,1,@idProducto);
        ROLLBACK; RETURN;
    END

    -- Evitar duplicados
    IF EXISTS (
        SELECT 1 
          FROM dbo.ProveedorProducto 
         WHERE idProveedor = @idProveedor 
           AND idProducto  = @idProducto
    )
    BEGIN
        RAISERROR('Relación Proveedor %d – Producto %d ya existe.',16,1,@idProveedor,@idProducto);
        ROLLBACK; RETURN;
    END

    INSERT INTO dbo.ProveedorProducto (idProveedor, idProducto, precioRef, activo)
    VALUES (@idProveedor, @idProducto, @precioRef, @activo);

    COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Ins_Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Ins_Servicio]
    @serNombre      VARCHAR(60),
    @serPrecioUnit  FLOAT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    INSERT INTO Servicio (serNombre, serPrecioUnit)
    VALUES (@serNombre, @serPrecioUnit);

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Ins_Vehiculo]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Ins_Vehiculo]
    @vhMatricula VARCHAR(60),
    @vhMarca     VARCHAR(60),
    @vhModelo    VARCHAR(60),
    @vhAño       INT,
    @vhColor     VARCHAR(60),
    @vhEstado    VARCHAR(50),
    @idCliente   INT,
    @idVehiculo  INT OUTPUT -- <- nuevo parámetro de salida
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1
        FROM Cliente
        WHERE idCliente = @idCliente
          AND clActivo = 1
    )
    BEGIN
        RAISERROR('Cliente %d no existe o está inactivo.', 16, 1, @idCliente);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    INSERT INTO Vehiculo (vhMatricula, vhMarca, vhModelo, vhAño, vhColor, vhEstado, idCliente)
    VALUES (@vhMatricula, @vhMarca, @vhModelo, @vhAño, @vhColor, @vhEstado, @idCliente);

    SET @idVehiculo = SCOPE_IDENTITY(); -- <- retorna el ID insertado

    COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoCl]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Mod_EstadoCl]
    @idCliente INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    DECLARE @actual BIT;

    -- Leer estado actual
    SELECT @actual = clActivo
      FROM Cliente
     WHERE idCliente = @idCliente;

    IF @actual IS NULL
    BEGIN
        RAISERROR('Cliente %d no existe.', 16, 1, @idCliente);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Alternar
    UPDATE Cliente
       SET clActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
     WHERE idCliente = @idCliente;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[mod_EstadoEmp]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[mod_EstadoEmp]
    @idEmpleado INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @actual BIT;

    -- Obtener el estado actual
    SELECT @actual = empActivo
      FROM Empleado
     WHERE idEmpleado = @idEmpleado;

    IF @actual IS NULL
    BEGIN
        RAISERROR('Empleado %d no existe.', 16, 1, @idEmpleado);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Alternar: si está activo, pasar a inactivo; si está inactivo, pasar a activo
    UPDATE Empleado
       SET empActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
     WHERE idEmpleado = @idEmpleado;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Mod_EstadoPack]
    @idPack INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @actual BIT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener estado actual
        SELECT @actual = pckActivo
          FROM PackReparacion
         WHERE idPack = @idPack;

        IF @actual IS NULL
        BEGIN
            RAISERROR('PackReparacion %d no existe.', 16, 1, @idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Alternar activación
        UPDATE PackReparacion
           SET pckActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
         WHERE idPack = @idPack;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoPres]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 1.3 Alternar Estado (Activo/Inactivo) de Presupuesto
-------------------------------
CREATE   PROCEDURE [dbo].[Mod_EstadoPres]
    @idPresupuesto INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @actual BIT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener estado actual
        SELECT @actual = preEstado
          FROM Presupuesto
         WHERE idPresupuesto = @idPresupuesto;

        IF @actual IS NULL
        BEGIN
            RAISERROR('Presupuesto %d no existe.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Alternar preEstado: 0 → 1, 1 → 0
        UPDATE Presupuesto
           SET preEstado = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
         WHERE idPresupuesto = @idPresupuesto;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoProd]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Mod_EstadoProd]
    @idProducto INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @actual BIT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener estado actual
        SELECT @actual = prActivo
          FROM dbo.Producto_Repuesto
         WHERE idProducto = @idProducto;

        IF @actual IS NULL
        BEGIN
            RAISERROR('Producto %d no existe.', 16, 1, @idProducto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Alternar el bit Activo: 1 → 0, 0 → 1
        UPDATE dbo.Producto_Repuesto
           SET prActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
         WHERE idProducto = @idProducto;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoProv]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-------------------------------
-- 3. Alternar Estado (Activo/Inactivo) de Proveedor
-------------------------------
CREATE   PROCEDURE [dbo].[Mod_EstadoProv]
    @idProveedor INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @actual BIT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @actual = pActivo
          FROM Proveedor
         WHERE idProveedor = @idProveedor;

        IF @actual IS NULL
        BEGIN
            RAISERROR('Proveedor %d no existe.', 16, 1, @idProveedor);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Proveedor
           SET pActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
         WHERE idProveedor = @idProveedor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoSer]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Mod_EstadoSer]
    @idServicio INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @actual BIT;

    SELECT @actual = serActivo
      FROM Servicio
     WHERE idServicio = @idServicio;

    IF @actual IS NULL
    BEGIN
        RAISERROR('Servicio %d no existe.', 16, 1, @idServicio);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE Servicio
       SET serActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
     WHERE idServicio = @idServicio;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Mod_EstadoVh]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Mod_EstadoVh]
    @idVehiculo INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @actual BIT;

    SELECT @actual = vhActivo
      FROM Vehiculo
     WHERE idVehiculo = @idVehiculo;

    IF @actual IS NULL
    BEGIN
        RAISERROR('Vehículo %d no existe.', 16, 1, @idVehiculo);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE Vehiculo
       SET vhActivo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END
     WHERE idVehiculo = @idVehiculo;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Toggle_Descuento]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Toggle_Descuento (activar/desactivar)
CREATE   PROCEDURE [dbo].[Toggle_Descuento]
    @IdDescuento INT,
    @Activar     BIT = NULL  -- NULL = alternar, 1 = activar, 0 = desactivar
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento)
        BEGIN
            RAISERROR('Descuento %d no existe.', 16, 1, @IdDescuento);
            ROLLBACK TRANSACTION; RETURN;
        END

        DECLARE @actual BIT;
        SELECT @actual = Activo FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento;

        DECLARE @nuevo BIT;
        IF @Activar IS NULL
            SET @nuevo = CASE WHEN @actual = 1 THEN 0 ELSE 1 END;
        ELSE
            SET @nuevo = @Activar;

        IF @nuevo = 1
        BEGIN
            -- Al activar validar solapamiento
            DECLARE @ID_PROD INT, @FI DATETIME, @FF DATETIME;
            SELECT @ID_PROD = ID_PROD, @FI = FechaInicio, @FF = FechaFin FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento;

            IF EXISTS (
                SELECT 1 FROM dbo.Descuentos d
                WHERE d.ID_PROD = @ID_PROD
                  AND d.IdDescuento <> @IdDescuento
                  AND ISNULL(d.Activo,1) = 1
                  AND d.FechaInicio < @FF
                  AND d.FechaFin > @FI
            )
            BEGIN
                RAISERROR('No se puede activar: existe otro descuento activo solapado para el mismo producto.', 16, 1);
                ROLLBACK TRANSACTION; RETURN;
            END
        END

        UPDATE dbo.Descuentos
        SET Activo = @nuevo
        WHERE IdDescuento = @IdDescuento;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Toggle_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Toggle_Producto]
    @ID_PROD INT,
    @Activar BIT = NULL  -- NULL = alternar, 1 = activar, 0 = desactivar
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que producto exista
        IF NOT EXISTS (
            SELECT 1 FROM dbo.Productos WHERE ID_PROD = @ID_PROD
        )
        BEGIN
            RAISERROR('Producto con ID %d no existe.', 16, 1, @ID_PROD);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DECLARE @NuevoEstado BIT;

        IF @Activar IS NULL
        BEGIN
            -- Alternar: si está activo (1) pasa a inactivo (0), y viceversa
            SELECT @NuevoEstado = CASE WHEN Activo = 1 THEN 0 ELSE 1 END
            FROM dbo.Productos
            WHERE ID_PROD = @ID_PROD;
        END
        ELSE
        BEGIN
            SET @NuevoEstado = @Activar;
        END

        -- Actualizar sólo si hay cambio
        IF EXISTS (
            SELECT 1 FROM dbo.Productos WHERE ID_PROD = @ID_PROD AND Activo <> @NuevoEstado
        )
        BEGIN
            UPDATE dbo.Productos
            SET Activo = @NuevoEstado
            WHERE ID_PROD = @ID_PROD;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Toggle_ProveedorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Toggle_ProveedorProducto]
    @idProveedor INT,
    @idProducto  INT,
    @activar     BIT = NULL    -- NULL = alternar, 1 = activar, 0 = desactivar
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    -- 1) Validar existencia de la relación
    IF NOT EXISTS (
        SELECT 1
          FROM dbo.ProveedorProducto
         WHERE idProveedor = @idProveedor
           AND idProducto  = @idProducto
    )
    BEGIN
        RAISERROR('No existe relación Proveedor %d – Producto %d.',16,1,@idProveedor,@idProducto);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 2) Determinar nuevo valor de activo
    DECLARE @nuevoActivo BIT;
    IF @activar IS NULL
    BEGIN
        -- alternar: si está 1 pasa a 0; si 0 pasa a 1
        SELECT @nuevoActivo = CASE WHEN activo = 1 THEN 0 ELSE 1 END
          FROM dbo.ProveedorProducto
         WHERE idProveedor = @idProveedor
           AND idProducto  = @idProducto;
    END
    ELSE
    BEGIN
        SET @nuevoActivo = @activar;
    END

    -- 3) Si ya está en el estado deseado, no hacemos nada
    IF NOT EXISTS (
        SELECT 1
          FROM dbo.ProveedorProducto
         WHERE idProveedor = @idProveedor
           AND idProducto  = @idProducto
           AND activo      = @nuevoActivo
    )
    BEGIN
        -- 4) Actualizar
        UPDATE dbo.ProveedorProducto
           SET activo = @nuevoActivo
         WHERE idProveedor = @idProveedor
           AND idProducto  = @idProducto;
    END

    COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Upd_Cliente]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Upd_Cliente]
    @idCliente    INT,
    @pNombre      VARCHAR(255),
    @pApellido    VARCHAR(255),
    @pTelefono    VARCHAR(20),
    @pCedula      VARCHAR(20),
    @clEmail      VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    DECLARE @idPersona INT;

    -- Obtener persona vinculada
    SELECT @idPersona = idPersona
      FROM Cliente
     WHERE idCliente = @idCliente;

    IF @idPersona IS NULL
    BEGIN
        RAISERROR('Cliente %d no existe.', 16, 1, @idCliente);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- 1. Actualizar Persona
    UPDATE Persona
       SET pNombre   = @pNombre,
           pApellido = @pApellido,
           pTelefono = @pTelefono,
           pCedula   = @pCedula
     WHERE idPersona = @idPersona;

    -- 2. Actualizar Cliente
    UPDATE Cliente
       SET clEmail = @clEmail
     WHERE idCliente = @idCliente;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_CompraProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [dbo].[Upd_CompraProducto]
  @idCompra   INT,
  @idProducto INT,
  @cproCantidad INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.Compra_Producto 
                   WHERE idCompra=@idCompra AND idProducto=@idProducto)
      BEGIN RAISERROR('DetalleCompra no existe.',16,1); ROLLBACK; RETURN; END

    -- 1) revertir stock antiguo
    DECLARE @oldQty INT;
    SELECT @oldQty = cproCantidad
      FROM dbo.Compra_Producto
     WHERE idCompra=@idCompra AND idProducto=@idProducto;

    UPDATE dbo.Producto_Repuesto
    SET prStock = prStock - @oldQty
    WHERE idProducto = @idProducto;

    -- 2) actualizar detalle
    UPDATE dbo.Compra_Producto
    SET cproCantidad = @cproCantidad
    WHERE idCompra=@idCompra AND idProducto=@idProducto;

    -- 3) sumar stock nuevo
    UPDATE dbo.Producto_Repuesto
    SET prStock = prStock + @cproCantidad
    WHERE idProducto = @idProducto;

  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Descuento]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Upd_Descuento
CREATE   PROCEDURE [dbo].[Upd_Descuento]
    @IdDescuento  INT,
    @Tipo         VARCHAR(20),
    @Valor        FLOAT,
    @FechaInicio  DATETIME,
    @FechaFin     DATETIME,
    @Activo       BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar existencia
        IF NOT EXISTS (SELECT 1 FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento)
        BEGIN
            RAISERROR('Descuento %d no existe.', 16, 1, @IdDescuento);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @FechaInicio >= @FechaFin
        BEGIN
            RAISERROR('FechaInicio debe ser anterior a FechaFin.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        DECLARE @tNorm VARCHAR(20) = UPPER(LTRIM(RTRIM(@Tipo)));
        IF @tNorm NOT IN ('PORCENTAJE','FIJO')
        BEGIN
            RAISERROR('Tipo inválido. Use ''Porcentaje'' o ''Fijo''.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @tNorm = 'PORCENTAJE' AND (@Valor < 0 OR @Valor > 100)
        BEGIN
            RAISERROR('Valor inválido para Porcentaje (0-100).', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        IF @tNorm = 'FIJO' AND (@Valor < 0)
        BEGIN
            RAISERROR('Valor inválido para Fijo (>=0).', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Obtener ID_PROD del descuento (para validaciones de solapamiento)
        DECLARE @ID_PROD INT;
        SELECT @ID_PROD = ID_PROD FROM dbo.Descuentos WHERE IdDescuento = @IdDescuento;

        -- Validar solapamiento con otros descuentos activos (excluyendo este)
        IF EXISTS (
            SELECT 1 FROM dbo.Descuentos d
            WHERE d.ID_PROD = @ID_PROD
              AND d.IdDescuento <> @IdDescuento
              AND ISNULL(d.Activo,1) = 1
              AND d.FechaInicio < @FechaFin
              AND d.FechaFin > @FechaInicio
        )
        BEGIN
            RAISERROR('Existe otro descuento activo solapado para este producto en el rango de fechas.', 16, 1);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Actualizar
        UPDATE dbo.Descuentos
        SET Tipo        = CASE WHEN @tNorm='PORCENTAJE' THEN 'Porcentaje' ELSE 'Fijo' END,
            Valor       = @Valor,
            FechaInicio = @FechaInicio,
            FechaFin    = @FechaFin,
            Activo      = @Activo
        WHERE IdDescuento = @IdDescuento;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_DetallePackProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_DetallePackProducto]
    @idDetalle    INT,
    @pdCantidad   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle:
        DECLARE @existePackProd BIT;
        SELECT @existePackProd = 1
          FROM DetallePack_Producto
         WHERE idDetalle = @idDetalle;
        IF @existePackProd IS NULL
        BEGIN
            RAISERROR('DetallePack_Producto %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idPack e idProducto actuales:
        DECLARE @idPack    INT,
                @idProd    INT;
        SELECT
            @idPack  = idPack,
            @idProd  = idProducto
          FROM DetallePack_Producto
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio unitario actualizado desde Producto_Repuesto:
        DECLARE @precioProducto FLOAT;
        SELECT @precioProducto = prPrecioVenta
          FROM Producto_Repuesto
         WHERE idProducto = @idProd
           AND prActivo = 1;
        IF @precioProducto IS NULL
        BEGIN
            RAISERROR('Producto_Repuesto %d no existe o está inactivo.', 16, 1, @idProd);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Actualizar cantidad (si vino) y forzar precio unitario desde Producto_Repuesto:
        UPDATE DetallePack_Producto
           SET pdCantidad   = COALESCE(@pdCantidad,   pdCantidad),
               pdPrecioUnit = @precioProducto
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_DetallePackServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_DetallePackServicio]
    @idDetalle   INT,
    @psCantidad  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle:
        DECLARE @existePackServ BIT;
        SELECT @existePackServ = 1
          FROM DetallePack_Servicio
         WHERE idDetalle = @idDetalle;
        IF @existePackServ IS NULL
        BEGIN
            RAISERROR('DetallePack_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idPack e idServicio actuales:
        DECLARE @idPack   INT,
                @idServ   INT;
        SELECT
            @idPack  = idPack,
            @idServ  = idServicio
          FROM DetallePack_Servicio
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio unitario actualizado desde Servicio:
        DECLARE @precioServicio FLOAT;
        SELECT @precioServicio = serPrecioUnit
          FROM Servicio
         WHERE idServicio = @idServ
           AND serActivo = 1;
        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServ);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Actualizar cantidad (si vino) y forzar precio unitario desde Servicio:
        UPDATE DetallePack_Servicio
           SET psCantidad   = COALESCE(@psCantidad,   psCantidad),
               psPrecioUnit = @precioServicio
         WHERE idDetalle = @idDetalle;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_DetallePagoOrden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_DetallePagoOrden]
  @idPagoOrden   INT,
  @metodoPago    VARCHAR(50)  = NULL,
  @observaciones VARCHAR(255) = NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;
    IF NOT EXISTS (SELECT 1 FROM dbo.DetallePagoOrden WHERE idPagoOrden=@idPagoOrden)
      BEGIN RAISERROR('PagoOrden %d no existe.',16,1,@idPagoOrden); ROLLBACK; RETURN; END

    UPDATE dbo.DetallePagoOrden
    SET metodoPago    = COALESCE(@metodoPago,    metodoPago),
        observaciones = COALESCE(@observaciones, observaciones)
    WHERE idPagoOrden = @idPagoOrden;
  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Empleado]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Upd_Empleado]
    @idEmpleado        INT,
    @pNombre           VARCHAR(255),
    @pApellido         VARCHAR(255),
    @pTelefono         VARCHAR(20),
    @pCedula           VARCHAR(20),
    @empUsuario        VARCHAR(20),
    @idRol             INT,
    @contrasena_texto  VARCHAR(128) = NULL  
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    DECLARE @idPersona INT;

    -- Obtener la persona asociada
    SELECT @idPersona = idPersona
      FROM Empleado
     WHERE idEmpleado = @idEmpleado;

    IF @idPersona IS NULL
    BEGIN
        RAISERROR('Empleado %d no existe.', 16, 1, @idEmpleado);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Actualizar datos en Persona
    UPDATE Persona
       SET pNombre   = @pNombre,
           pApellido = @pApellido,
           pTelefono = @pTelefono,
           pCedula   = @pCedula
     WHERE idPersona = @idPersona;

    -- Actualizar datos en Empleado
    UPDATE Empleado
       SET empUsuario       = @empUsuario,
           idRol            = @idRol,
           contrasena_texto = COALESCE(@contrasena_texto, contrasena_texto)
     WHERE idEmpleado = @idEmpleado;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Orden]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Upd_Orden]
    @idOrden           INT,
    @orEstado          VARCHAR(30),
    @orTipo            VARCHAR(50),
    @orFechaEntrega    DATETIME,
    @orFechaFin        DATETIME    = NULL,
    @orFalla           VARCHAR(MAX),
    @idPresupuesto     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia de la Orden
        IF NOT EXISTS (SELECT 1 FROM Orden WHERE idOrden = @idOrden)
        BEGIN
            RAISERROR('Orden %d no existe.', 16, 1, @idOrden);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 2) (opcional) Validar transición de estado
        DECLARE @oldEstado VARCHAR(30);
        SELECT @oldEstado = orEstado FROM dbo.Orden WHERE idOrden = @idOrden;
        IF @oldEstado = 'Entregado' AND @orEstado <> 'Entregado'
        BEGIN
            RAISERROR(
              'No se puede retroceder de estado entregado (actual=%s).',
              16,1,@oldEstado);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 3) Validar existencia del Presupuesto (si no viene, lo ignora)
        IF @idPresupuesto IS NOT NULL
           AND NOT EXISTS (SELECT 1 FROM Presupuesto WHERE idPresupuesto = @idPresupuesto)
        BEGIN
            RAISERROR('Presupuesto %d no existe.', 16, 1, @idPresupuesto);
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 4) Actualizar solo los campos que pueden cambiar
        UPDATE Orden
           SET orEstado       = @orEstado,
               orTipo         = @orTipo,
               orFechaEntrega = @orFechaEntrega,
               orFechaFin     = @orFechaFin,
               orFalla        = @orFalla,
               idPresupuesto  = @idPresupuesto
         WHERE idOrden = @idOrden;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_OrdenDetEntrega]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------
-- 2.2 Actualizar DetalleEntrega
-------------------------------
CREATE   PROCEDURE [dbo].[Upd_OrdenDetEntrega]
    @idEntrega            INT,
    @recibido_por         VARCHAR(100) = NULL,
    @telefono_recibe      VARCHAR(20)  = NULL,
    @es_propietario       BIT          = NULL,
    @entregado_por        INT          = NULL,
    @fecha_entrega        DATETIME     = NULL,
    @orden_entrega        VARCHAR(50)  = NULL,
    @mecanico_responsable INT          = NULL,
    @informe_tecnico      VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Verificar existencia del detalle
        IF NOT EXISTS (
            SELECT 1
              FROM OrdenDetalle_Entrega
             WHERE idEntrega = @idEntrega
        )
        BEGIN
            RAISERROR('DetalleEntrega %d no existe.', 16, 1, @idEntrega);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Si cambió entregado_por, validar que el empleado exista y esté activo
        IF @entregado_por IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                  FROM Empleado
                 WHERE idEmpleado = @entregado_por
                   AND empActivo = 1
            )
            BEGIN
                RAISERROR('Empleado (entregado_por) %d no existe o está inactivo.', 16, 1, @entregado_por);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- 3. Si cambió mecanico_responsable, validar que el empleado exista y esté activo
        IF @mecanico_responsable IS NOT NULL
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                  FROM Empleado
                 WHERE idEmpleado = @mecanico_responsable
                   AND empActivo = 1
            )
            BEGIN
                RAISERROR('Empleado (mecanico_responsable) %d no existe o está inactivo.', 16, 1, @mecanico_responsable);
                ROLLBACK TRANSACTION;
                RETURN;
            END
        END

        -- 4. Actualizar solo los campos proporcionados
        UPDATE OrdenDetalle_Entrega
           SET recibido_por         = COALESCE(@recibido_por,         recibido_por),
               telefono_recibe      = COALESCE(@telefono_recibe,      telefono_recibe),
               es_propietario       = COALESCE(@es_propietario,       es_propietario),
               entregado_por        = COALESCE(@entregado_por,        entregado_por),
               fecha_entrega        = COALESCE(@fecha_entrega,        fecha_entrega),
               orden_entrega        = COALESCE(@orden_entrega,        orden_entrega),
               mecanico_responsable = COALESCE(@mecanico_responsable, mecanico_responsable),
               informe_tecnico      = COALESCE(@informe_tecnico,      informe_tecnico)
         WHERE idEntrega = @idEntrega;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_OrdenDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** 1) Actualizar Detalle de Pack ******/
CREATE PROCEDURE [dbo].[Upd_OrdenDetPack]
  @idDetalle   INT,
  @opCantidad  INT = NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    ----------------------------------------------------
    -- 1) Verificar existencia del detalle de pack
    ----------------------------------------------------
    IF NOT EXISTS (
      SELECT 1 FROM dbo.OrdenDetalle_Pack WHERE idDetalle = @idDetalle
    )
    BEGIN
      RAISERROR('OrdenDetalle_Pack %d no existe.',16,1,@idDetalle);
      ROLLBACK TRANSACTION; RETURN;
    END

    ----------------------------------------------------
    -- 2) Obtener datos actuales: idOrden, idPack, cant y precio viejo
    ----------------------------------------------------
    DECLARE @idOrd INT, @idPack INT,
            @cantVieja INT, @precioViejo FLOAT;
    SELECT
      @idOrd       = idOrden,
      @idPack      = idPack,
      @cantVieja   = opCantidad,
      @precioViejo = opPrecioPack
    FROM dbo.OrdenDetalle_Pack
    WHERE idDetalle = @idDetalle;

    ----------------------------------------------------
    -- 3) Verificar que el pack siga activo y obtener precio actual
    ----------------------------------------------------
    DECLARE @precioPack FLOAT;
    SELECT @precioPack = pckPrecio
      FROM dbo.PackReparacion
     WHERE idPack = @idPack
       AND pckActivo = 1;
    IF @precioPack IS NULL
    BEGIN
      RAISERROR('Pack %d no existe o está inactivo.',16,1,@idPack);
      ROLLBACK TRANSACTION; RETURN;
    END

    ----------------------------------------------------
    -- 4) Actualizar cantidad y precio en el detalle
    ----------------------------------------------------
    UPDATE dbo.OrdenDetalle_Pack
      SET opCantidad    = COALESCE(@opCantidad, @cantVieja),
          opPrecioPack  = @precioPack
    WHERE idDetalle = @idDetalle;

    ----------------------------------------------------
    -- 5) Ajustar orCostoTotal en Orden
    ----------------------------------------------------
    -- restar el importe viejo
    UPDATE dbo.Orden
      SET orCostoTotal = orCostoTotal - (@cantVieja * @precioViejo)
    WHERE idOrden = @idOrd;

    -- sumar el nuevo importe
    DECLARE @cantNueva INT = COALESCE(@opCantidad, @cantVieja);
    UPDATE dbo.Orden
      SET orCostoTotal = orCostoTotal + (@cantNueva * @precioPack)
    WHERE idOrden = @idOrd;

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
  END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_OrdenDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** 2) Actualizar Detalle de Producto ******/
CREATE PROCEDURE [dbo].[Upd_OrdenDetProducto]
  @idDetalle  INT,
  @opCantidad INT = NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRANSACTION;

    ----------------------------------------------------
    -- 1) Verificar existencia del detalle de producto
    ----------------------------------------------------
    IF NOT EXISTS (
      SELECT 1 FROM dbo.OrdenDetalle_Producto WHERE idDetalle = @idDetalle
    )
    BEGIN
      RAISERROR('OrdenDetalle_Producto %d no existe.',16,1,@idDetalle);
      ROLLBACK TRANSACTION; RETURN;
    END

    ----------------------------------------------------
    -- 2) Obtener datos actuales: idOrden, idProducto, cant y precio viejo
    ----------------------------------------------------
    DECLARE @idOrd INT, @idProd INT,
            @cantVieja INT, @precioViejo FLOAT;
    SELECT
      @idOrd       = idOrden,
      @idProd      = idProducto,
      @cantVieja   = opCantidad,
      @precioViejo = opPrecioUnit
    FROM dbo.OrdenDetalle_Producto
    WHERE idDetalle = @idDetalle;

    ----------------------------------------------------
    -- 3) Verificar existencia y estado del producto
    ----------------------------------------------------
    DECLARE @precioProd FLOAT, @stockActual INT;
    SELECT 
      @precioProd   = prPrecioVenta,
      @stockActual  = prStock
    FROM dbo.Producto_Repuesto
    WHERE idProducto = @idProd
      AND prActivo = 1;
    IF @precioProd IS NULL
    BEGIN
      RAISERROR('Producto %d no existe o está inactivo.',16,1,@idProd);
      ROLLBACK TRANSACTION; RETURN;
    END

    ----------------------------------------------------
    -- 4) Validar stock si aumentamos cantidad
    ----------------------------------------------------
    DECLARE @cantNueva INT = COALESCE(@opCantidad, @cantVieja);
    IF @cantNueva > @cantVieja
    BEGIN
      IF (@cantNueva - @cantVieja) > @stockActual
      BEGIN
        RAISERROR(
          'No hay suficiente stock para el producto %d. Solo quedan %d.', 
          16,1,@idProd,@stockActual
        );
        ROLLBACK TRANSACTION; RETURN;
      END
    END

    ----------------------------------------------------
    -- 5) Actualizar detalle: cantidad y precio
    ----------------------------------------------------
    UPDATE dbo.OrdenDetalle_Producto
      SET opCantidad   = @cantNueva,
          opPrecioUnit = @precioProd
    WHERE idDetalle = @idDetalle;

    ----------------------------------------------------
    -- 6) Ajustar orCostoTotal en Orden
    ----------------------------------------------------
    -- restar el viejo
    UPDATE dbo.Orden
      SET orCostoTotal = orCostoTotal - (@cantVieja * @precioViejo)
    WHERE idOrden = @idOrd;

    -- sumar el nuevo
    UPDATE dbo.Orden
      SET orCostoTotal = orCostoTotal + (@cantNueva * @precioProd)
    WHERE idOrden = @idOrd;

    ----------------------------------------------------
    -- 7) Ajustar stock en Producto_Repuesto
    ----------------------------------------------------
    -- restamos el incremento
    UPDATE dbo.Producto_Repuesto
      SET prStock = prStock - (@cantNueva - @cantVieja)
    WHERE idProducto = @idProd;

    COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
  END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_OrdenDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_OrdenDetServicio]
    @idDetalle    INT,
    @opCantidad   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle de servicio:
        DECLARE @existeDetServ BIT;
        SELECT @existeDetServ = 1
          FROM OrdenDetalle_Servicio
         WHERE idDetalle = @idDetalle;
        IF @existeDetServ IS NULL
        BEGIN
            RAISERROR('OrdenDetalle_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idOrden e idServicio:
        DECLARE @idOrd    INT,
                @idServ   INT;
        SELECT
            @idOrd  = idOrden,
            @idServ = idServicio
          FROM OrdenDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio actual del servicio:
        DECLARE @precioServicio FLOAT;
        SELECT @precioServicio = serPrecioUnit
          FROM Servicio
         WHERE idServicio = @idServ
           AND serActivo = 1;
        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServ);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Calcular diferencia en costo total de la orden:
        DECLARE @cantVieja   INT,
                @precioViejo FLOAT;
        SELECT 
            @cantVieja   = opCantidad,
            @precioViejo = opPrecioUnit
          FROM OrdenDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        -- 5) Actualizar detalle: nueva cantidad y precio recargado:
        UPDATE OrdenDetalle_Servicio
           SET opCantidad   = COALESCE(@opCantidad,   opCantidad),
               opPrecioUnit = @precioServicio
         WHERE idDetalle = @idDetalle;

        -- 6) Ajustar el costo total en 'Orden':
        --    Primero, restar lo que existía antes:
        UPDATE Orden
        SET orCostoTotal = orCostoTotal - (@cantVieja * @precioViejo)
        WHERE idOrden = @idOrd;

        --    Luego sumar con la nueva cantidad/precio:
        DECLARE @cantNueva INT = COALESCE(@opCantidad, @cantVieja);
        UPDATE Orden
        SET orCostoTotal = orCostoTotal + (@cantNueva * @precioServicio)
        WHERE idOrden = @idOrd;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_PackReparacion]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Upd_PackReparacion]
  @idPack         INT,
  @pckNombre      VARCHAR(60),
  @pckDescripcion VARCHAR(250) = NULL
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;
    IF NOT EXISTS (SELECT 1 FROM dbo.PackReparacion WHERE idPack=@idPack)
      BEGIN RAISERROR('Pack %d no existe.',16,1,@idPack); ROLLBACK; RETURN; END

    IF EXISTS (
      SELECT 1 FROM dbo.PackReparacion 
       WHERE pckNombre=@pckNombre 
         AND pckActivo=1 
         AND idPack<>@idPack
    )
      BEGIN RAISERROR('Ya hay otro pack activo con ese nombre.',16,1); ROLLBACK; RETURN; END

    UPDATE dbo.PackReparacion
    SET pckNombre      = @pckNombre,
        pckDescripcion = @pckDescripcion
    WHERE idPack = @idPack;
  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Presupuesto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_Presupuesto]
  @idPresupuesto    INT,
  @preFalla         VARCHAR(MAX),
  @idVehiculo       INT=null, 
  @idCliente        INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRANSACTION;
    IF NOT EXISTS (SELECT 1 FROM dbo.Presupuesto WHERE idPresupuesto=@idPresupuesto)
      BEGIN RAISERROR('Presupuesto %d no existe.',16,1,@idPresupuesto); ROLLBACK; RETURN; END

	  DECLARE @estadoActual BIT;
SELECT @estadoActual = preEstado
  FROM dbo.Presupuesto
 WHERE idPresupuesto = @idPresupuesto;
IF @estadoActual = 1
BEGIN
  RAISERROR('No se puede modificar un presupuesto aprobado.',16,1);
  ROLLBACK;
  RETURN;
END

IF NOT EXISTS (
  SELECT 1
    FROM dbo.Cliente
   WHERE idCliente = @idCliente
     AND clActivo = 1
)
BEGIN
  RAISERROR('Cliente %d inválido o inactivo.',16,1,@idCliente);
  ROLLBACK;
  RETURN;
END

IF @idVehiculo IS NOT NULL
  AND NOT EXISTS (
    SELECT 1
      FROM dbo.Vehiculo
     WHERE idVehiculo = @idVehiculo
       AND vhActivo   = 1
  )
BEGIN
  RAISERROR('Vehículo %d inválido o inactivo.',16,1,@idVehiculo);
  ROLLBACK;
  RETURN;
END


    UPDATE dbo.Presupuesto
    SET --preFechaCreacion = GETDATE(), no creo que se deba modificar la fecha de creacion
        preFalla         = @preFalla,
        idVehiculo       = @idVehiculo,
        idCliente        = @idCliente
    WHERE idPresupuesto = @idPresupuesto;
  COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_PresupuestoDetPack]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_PresupuestoDetPack]
    @idDetalle     INT,
    @ppCantidad    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle:
        DECLARE @existeDetPack BIT;
        SELECT @existeDetPack = 1
          FROM PresupuestoDetalle_Pack
         WHERE idDetalle = @idDetalle;
        IF @existeDetPack IS NULL
        BEGIN
            RAISERROR('PresupuestoDetalle_Pack %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idPresupuesto e idPack:
        DECLARE @idPres   INT,
                @idPack   INT;
        SELECT
            @idPres  = idPresupuesto,
            @idPack  = idPack
          FROM PresupuestoDetalle_Pack
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio actual del pack:
        DECLARE @precioPack FLOAT;
        SELECT @precioPack = pckPrecio
          FROM PackReparacion
         WHERE idPack = @idPack
           AND pckActivo = 1;
        IF @precioPack IS NULL
        BEGIN
            RAISERROR('PackReparacion %d no existe o está inactivo.', 16, 1, @idPack);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Calcular diferencia en preCostoTotal:
        DECLARE @cantVieja   INT,
                @precioViejo FLOAT;
        SELECT 
            @cantVieja   = ppCantidad,
            @precioViejo = ppPrecioPack
          FROM PresupuestoDetalle_Pack
         WHERE idDetalle = @idDetalle;

        -- 5) Actualizar detalle: nueva cantidad y precio recargado:
        UPDATE PresupuestoDetalle_Pack
           SET ppCantidad    = COALESCE(@ppCantidad,   ppCantidad),
               ppPrecioPack  = @precioPack
         WHERE idDetalle = @idDetalle;

        -- 6) Ajustar el preCostoTotal en 'Presupuesto':
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal - (@cantVieja * @precioViejo)
        WHERE idPresupuesto = @idPres;

        DECLARE @cantNueva INT = COALESCE(@ppCantidad, @cantVieja);
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal + (@cantNueva * @precioPack)
        WHERE idPresupuesto = @idPres;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_PresupuestoDetProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_PresupuestoDetProducto]
    @idDetalle     INT,
    @ppCantidad    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle:
        DECLARE @existeDetProd BIT;
        SELECT @existeDetProd = 1
          FROM PresupuestoDetalle_Producto
         WHERE idDetalle = @idDetalle;
        IF @existeDetProd IS NULL
        BEGIN
            RAISERROR('PresupuestoDetalle_Producto %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idPresupuesto e idProducto:
        DECLARE @idPres   INT,
                @idProd   INT;
        SELECT
            @idPres  = idPresupuesto,
            @idProd  = idProducto
          FROM PresupuestoDetalle_Producto
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio actual del producto:
        DECLARE @precioProducto FLOAT;
        SELECT @precioProducto = prPrecioVenta
          FROM Producto_Repuesto
         WHERE idProducto = @idProd
           AND prActivo = 1;
        IF @precioProducto IS NULL
        BEGIN
            RAISERROR('Producto %d no existe o está inactivo.', 16, 1, @idProd);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Calcular diferencia en preCostoTotal:
        DECLARE @cantVieja   INT,
                @precioViejo FLOAT;
        SELECT 
            @cantVieja   = ppCantidad,
            @precioViejo = ppPrecioUnit
          FROM PresupuestoDetalle_Producto
         WHERE idDetalle = @idDetalle;

        -- 5) Actualizar detalle: nueva cantidad y precio recargado:
        UPDATE PresupuestoDetalle_Producto
           SET ppCantidad   = COALESCE(@ppCantidad,   ppCantidad),
               ppPrecioUnit = @precioProducto
         WHERE idDetalle = @idDetalle;

        -- 6) Ajustar el preCostoTotal en 'Presupuesto':
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal - (@cantVieja * @precioViejo)
        WHERE idPresupuesto = @idPres;

        DECLARE @cantNueva INT = COALESCE(@ppCantidad, @cantVieja);
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal + (@cantNueva * @precioProducto)
        WHERE idPresupuesto = @idPres;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_PresupuestoDetServicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_PresupuestoDetServicio]
    @idDetalle     INT,
    @psCantidad    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1) Verificar existencia del detalle:
        DECLARE @existeDetServ BIT;
        SELECT @existeDetServ = 1
          FROM PresupuestoDetalle_Servicio
         WHERE idDetalle = @idDetalle;
        IF @existeDetServ IS NULL
        BEGIN
            RAISERROR('PresupuestoDetalle_Servicio %d no existe.', 16, 1, @idDetalle);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2) Obtener idPresupuesto e idServicio:
        DECLARE @idPres   INT,
                @idServ   INT;
        SELECT
            @idPres  = idPresupuesto,
            @idServ  = idServicio
          FROM PresupuestoDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        -- 3) Obtener el precio actual del servicio:
        DECLARE @precioServicio FLOAT;
        SELECT @precioServicio = serPrecioUnit
          FROM Servicio
         WHERE idServicio = @idServ
           AND serActivo = 1;
        IF @precioServicio IS NULL
        BEGIN
            RAISERROR('Servicio %d no existe o está inactivo.', 16, 1, @idServ);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4) Calcular diferencia en preCostoTotal:
        DECLARE @cantVieja   INT,
                @precioViejo FLOAT;
        SELECT 
            @cantVieja   = psCantidad,
            @precioViejo = psPrecioUnit
          FROM PresupuestoDetalle_Servicio
         WHERE idDetalle = @idDetalle;

        -- 5) Actualizar detalle: nueva cantidad y precio recargado:
        UPDATE PresupuestoDetalle_Servicio
           SET psCantidad   = COALESCE(@psCantidad,   psCantidad),
               psPrecioUnit = @precioServicio
         WHERE idDetalle = @idDetalle;

        -- 6) Ajustar el preCostoTotal en 'Presupuesto':
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal - (@cantVieja * @precioViejo)
        WHERE idPresupuesto = @idPres;

        DECLARE @cantNueva INT = COALESCE(@psCantidad, @cantVieja);
        UPDATE Presupuesto
        SET preCostoTotal = preCostoTotal + (@cantNueva * @precioServicio)
        WHERE idPresupuesto = @idPres;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Producto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Upd_Producto]
    @idProducto     INT,
    @prNombre       VARCHAR(100),
    @prMarca        VARCHAR(100) = NULL,
    @prCategoria    VARCHAR(100) = NULL,
    @prCosto        FLOAT        = NULL,
    @prPrecioVenta  FLOAT        = NULL,
    @prStock        INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Verificar que exista producto en la tabla correcta
        IF NOT EXISTS (SELECT 1 FROM dbo.Producto_Repuesto WHERE idProducto = @idProducto)
        BEGIN
            RAISERROR('El producto con ID %d no existe.', 16, 1, @idProducto);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE dbo.Producto_Repuesto
        SET prNombre     = @prNombre,
            prMarca      = @prMarca,
            prCategoria  = @prCategoria,
            prCosto      = @prCosto,
            prPrecioVenta= @prPrecioVenta,
            prStock      = COALESCE(@prStock, prStock)
        WHERE idProducto = @idProducto;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Proveedor]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Upd_Proveedor]
    @idProveedor INT,
    @pNombre     VARCHAR(100),
    @pTelefono   VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
              FROM Proveedor
             WHERE idProveedor = @idProveedor
        )
        BEGIN
            RAISERROR('Proveedor %d no existe.', 16, 1, @idProveedor);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
              FROM Proveedor
             WHERE pNombre = @pNombre
               AND pActivo = 1
               AND idProveedor <> @idProveedor
        )
        BEGIN
            RAISERROR('Otro proveedor activo ya utiliza el nombre %s.', 16, 1, @pNombre);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Proveedor
           SET pNombre   = @pNombre,
               pTelefono = @pTelefono
         WHERE idProveedor = @idProveedor;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_ProveedorProducto]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[Upd_ProveedorProducto]
    @idProveedor INT,
    @idProducto  INT,
    @precioRef   FLOAT = NULL,
    @activo      BIT   = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1 
          FROM dbo.ProveedorProducto 
         WHERE idProveedor = @idProveedor 
           AND idProducto  = @idProducto
    )
    BEGIN
        RAISERROR('No existe relación Proveedor %d – Producto %d.',16,1,@idProveedor,@idProducto);
        ROLLBACK; RETURN;
    END

    UPDATE dbo.ProveedorProducto
       SET precioRef = COALESCE(@precioRef, precioRef),
           activo    = COALESCE(@activo,    activo)
     WHERE idProveedor = @idProveedor
       AND idProducto  = @idProducto;

    COMMIT TRANSACTION;
END
GO
/****** Object:  StoredProcedure [dbo].[Upd_Servicio]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_Servicio]
    @idServicio     INT,
    @serNombre      VARCHAR(60),
    @serPrecioUnit  FLOAT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM Servicio WHERE idServicio = @idServicio)
    BEGIN
        RAISERROR('Servicio %d no existe.', 16, 1, @idServicio);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE Servicio
       SET serNombre     = @serNombre,
           serPrecioUnit = @serPrecioUnit
     WHERE idServicio = @idServicio;

    COMMIT TRANSACTION;
END;
GO
/****** Object:  StoredProcedure [dbo].[Upd_Vehiculo]    Script Date: 20/5/2026 11:00:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[Upd_Vehiculo]
    @idVehiculo  INT,
    @vhMatricula VARCHAR(60),
    @vhMarca     VARCHAR(60),
    @vhModelo    VARCHAR(60),
    @vhAño       INT,
    @vhColor     VARCHAR(60),
    @vhEstado    VARCHAR(50),
    @idCliente   INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1
        FROM Vehiculo
        WHERE idVehiculo = @idVehiculo
    )
    BEGIN
        RAISERROR('Vehículo %d no existe.', 16, 1, @idVehiculo);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1
        FROM Cliente
        WHERE idCliente = @idCliente
          AND clActivo = 1
    )
    BEGIN
        RAISERROR('Cliente %d no existe o está inactivo.', 16, 1, @idCliente);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    UPDATE Vehiculo
    SET vhMatricula = @vhMatricula,
        vhMarca     = @vhMarca,
        vhModelo    = @vhModelo,
        vhAño       = @vhAño,
        vhColor     = @vhColor,
        vhEstado    = @vhEstado,
        idCliente   = @idCliente
    WHERE idVehiculo = @idVehiculo;

    COMMIT TRANSACTION;
END;
GO
USE [master]
GO
ALTER DATABASE [BD_Taller] SET  READ_WRITE 
GO
