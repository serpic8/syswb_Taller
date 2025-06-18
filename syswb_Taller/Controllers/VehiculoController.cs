using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class VehiculoController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public VehiculoController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista(int idCliente)
        {
            ViewBag.IdCliente = idCliente;
            var vehiculos = new List<dynamic>();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_Vehiculos";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Matricula", DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Marca", DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Activo", DBNull.Value));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                vehiculos.Add(new
                {
                    IdVehiculo = reader.GetInt32(0),
                    vhMatricula = reader.GetString(1),
                    vhMarca = reader.GetString(2),
                    vhModelo = reader.GetString(3),
                    vhAño = reader.GetInt32(4),
                    vhColor = reader.GetString(5),
                    vhActivo = reader.GetBoolean(6)
                });
            }

            return View(vehiculos);
        }



        // GET: /Vehiculo/Crear/5
        // Dentro de VehiculoController:

        // GET: /Vehiculo/SeleccionarCliente
        [HttpGet]
        public async Task<IActionResult> SeleccionarCliente(string nombre = null, bool? activo = null)
        {
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;

            var clientes = new List<dynamic>();
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_Clientes";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Activo", (object)activo ?? DBNull.Value));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                clientes.Add(new
                {
                    IdCliente = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Telefono = reader.GetString(3)
                });
            }

            return View(clientes);
        }

        // Modificamos tu Crear GET para forzar selección:
        [HttpGet]
        public async Task<IActionResult> Crear(int? idCliente)
        {
            if (idCliente == null)
            {
                TempData["Error"] = "Por favor selecciona primero un Cliente.";
                return RedirectToAction(nameof(SeleccionarCliente));
            }

            ViewBag.IdCliente = idCliente.Value;
            ViewBag.Productos = await _bdTallerContext.ProductoRepuestos.Where(p => p.PrActivo).AsNoTracking().ToListAsync();
            ViewBag.Servicios = await _bdTallerContext.Servicios.Where(s => s.SerActivo).AsNoTracking().ToListAsync();
            ViewBag.Detalles = new List<dynamic>();
            ViewBag.CostoTotal = 0.0;

            return View();
        }


        // POST: /Vehiculo/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            string vhMatricula,
            string vhMarca,
            string vhModelo,
            int vhAño,
            string vhColor,
            string vhEstado,
            int idCliente)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_Vehiculo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@vhMatricula", vhMatricula));
            cmd.Parameters.Add(new SqlParameter("@vhMarca", vhMarca));
            cmd.Parameters.Add(new SqlParameter("@vhModelo", vhModelo));
            cmd.Parameters.Add(new SqlParameter("@vhAño", vhAño));
            cmd.Parameters.Add(new SqlParameter("@vhColor", vhColor));
            cmd.Parameters.Add(new SqlParameter("@vhEstado", vhEstado));
            cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista", "Vehiculo", new { id = idCliente });
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, int idCliente)
        {
            // 'id' ahora es el idVehiculo
            int idVehiculo = id;

            dynamic veh = null;
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            SELECT vhMatricula, vhMarca, vhModelo, vhAño, vhColor, vhEstado, idCliente
            FROM Vehiculo
            WHERE idVehiculo = @idVehiculo";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@idVehiculo", idVehiculo));

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                veh = new
                {
                    IdVehiculo = idVehiculo,
                    Matricula = reader.GetString(0),
                    Marca = reader.GetString(1),
                    Modelo = reader.GetString(2),
                    Año = reader.GetInt32(3),
                    Color = reader.GetString(4),
                    Estado = reader.GetString(5),
                    IdCliente = reader.GetInt32(6)
                };
            }

            if (veh == null) return NotFound();
            return View(veh);
        }

        // POST: /Vehiculo/Editar
        // POST: /Vehiculo/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
             int idVehiculo,
             string vhMatricula,
             string vhMarca,
             string vhModelo,
             int vhAño,
             string vhColor,
             string vhEstado,
             int idCliente)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_Vehiculo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idVehiculo", idVehiculo));
            cmd.Parameters.Add(new SqlParameter("@vhMatricula", vhMatricula));
            cmd.Parameters.Add(new SqlParameter("@vhMarca", vhMarca));
            cmd.Parameters.Add(new SqlParameter("@vhModelo", vhModelo));
            cmd.Parameters.Add(new SqlParameter("@vhAño", vhAño));
            cmd.Parameters.Add(new SqlParameter("@vhColor", vhColor));
            cmd.Parameters.Add(new SqlParameter("@vhEstado", vhEstado));
            cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));

            await cmd.ExecuteNonQueryAsync();

            // ¡Fíjate en la coma y las llaves!
            return RedirectToAction("Lista", new { idCliente });
        }




        // POST: /Vehiculo/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int idVehiculo, int idCliente)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Mod_EstadoVh";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idVehiculo", idVehiculo));

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista", new { idCliente });
        }
    }
}
