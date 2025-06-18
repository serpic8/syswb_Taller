using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class ServicioController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public ServicioController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista(string nombre = null, bool? activo = null)
        {
            var servicios = new List<Servicio>();

            var conn = _bdTallerContext.Database.GetDbConnection();

            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_Servicios";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", (object)activo ?? DBNull.Value));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        servicios.Add(new Servicio
                        {
                            IdServicio = reader.GetInt32(0),
                            SerNombre = reader.GetString(1),
                            SerPrecioUnit = reader.GetDouble(2),
                            SerActivo = reader.GetBoolean(3)
                        });
                    }
                }
            }

            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;

            return View(servicios);
        }




        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // -------------------------------------------
        // POST: Insertar Servicio
        // -------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string serNombre, double serPrecioUnit)
        {
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Ins_Servicio";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@serNombre", serNombre));
                cmd.Parameters.Add(new SqlParameter("@serPrecioUnit", serPrecioUnit));

                await cmd.ExecuteNonQueryAsync();
            }

            return RedirectToAction("Lista");
        }

        // -------------------------------------------
        // GET: Editar Servicio
        // -------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            dynamic servicio = null;

            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT idServicio, serNombre, serPrecioUnit FROM Servicio WHERE idServicio = @id";
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        servicio = new
                        {
                            IdServicio = reader.GetInt32(0),
                            SerNombre = reader.GetString(1),
                            SerPrecioUnit = reader.GetDouble(2)
                        };
                    }
                }
            }

            if (servicio == null)
                return NotFound();

            ViewBag.Servicio = servicio;
            return View();
        }

        // -------------------------------------------
        // POST: Actualizar Servicio
        // -------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int idServicio, string serNombre, double serPrecioUnit)
        {
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Upd_Servicio";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));
                cmd.Parameters.Add(new SqlParameter("@serNombre", serNombre));
                cmd.Parameters.Add(new SqlParameter("@serPrecioUnit", serPrecioUnit));

                await cmd.ExecuteNonQueryAsync();
            }

            return RedirectToAction("Lista");
        }

        // -------------------------------------------
        // POST: Modificar Estado (Activo/Inactivo)
        // -------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int idServicio)
        {
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Mod_EstadoSer";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));

                await cmd.ExecuteNonQueryAsync();
            }

            return RedirectToAction("Lista");
        }

    }
}
