using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class ClienteController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public ClienteController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        // GET: /Cliente/Lista
        [HttpGet]
        public async Task<IActionResult> Lista(string nombre = null, bool? activo = null)
        {
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
                    Telefono = reader.GetString(3),
                    OrdenesRealizadas = reader.GetInt32(4),
                    Activo = reader.GetBoolean(5)
                });
            }

            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;
            return View(clientes);
        }

        // GET: /Cliente/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Cliente/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            string pNombre,
            string pApellido,
            string pTelefono,
            string pCedula,
            string clEmail)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_Cliente";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
            cmd.Parameters.Add(new SqlParameter("@pApellido", pApellido));
            cmd.Parameters.Add(new SqlParameter("@pTelefono", pTelefono));
            cmd.Parameters.Add(new SqlParameter("@pCedula", pCedula));
            cmd.Parameters.Add(new SqlParameter("@clEmail", clEmail));

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista");
        }

        // GET: /Cliente/Editar/5
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            dynamic cliente = null;
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            // 1) Traer datos del cliente (ya lo tienes)
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
            SELECT c.idCliente,
                   p.pNombre,
                   p.pApellido,
                   p.pTelefono,
                   p.pCedula,
                   c.clEmail
              FROM Cliente c
              JOIN Persona p ON p.idPersona = c.idPersona
             WHERE c.idCliente = @id";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@id", id));

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    cliente = new
                    {
                        IdCliente = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        Telefono = reader.GetString(3),
                        Cedula = reader.GetString(4),
                        Email = reader.GetString(5)
                    };
                }
            }

            if (cliente == null)
                return NotFound();

            ViewBag.Cliente = cliente;

            // 2) Traer vehículos del cliente
            var vehiculos = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_VehiculosPorCliente";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCliente", id));
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vehiculos.Add(new
                    {
                        Matricula = reader.GetString(0),
                        Color = reader.GetString(1),
                        Marca = reader.GetString(2)
                    });
                }
            }
            ViewBag.Vehiculos = vehiculos;

            // 3) Traer órdenes del cliente (sin filtros, tú puedes exponer parámetros de fecha/estado)
            var ordenes = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_OrdenesPorCliente";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCliente", id));
                cmd.Parameters.Add(new SqlParameter("@fechaDesde", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@fechaHasta", DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@orEstado", DBNull.Value));
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ordenes.Add(new
                    {
                        IdOrden = reader.GetInt32(0),
                        FechaCreacion = reader.GetDateTime(1),
                        Estado = reader.GetString(2),
                        Tipo = reader.GetString(3),
                        FechaEntrega = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                        FechaFin = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                        CostoTotal = reader.GetDouble(6),
                        VehiculoMatric = reader.GetString(7),
                        VehiculoMarca = reader.GetString(8)
                    });
                }
            }
            ViewBag.Ordenes = ordenes;

            return View();
        }


        // POST: /Cliente/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int idCliente,
            string pNombre,
            string pApellido,
            string pTelefono,
            string pCedula,
            string clEmail)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_Cliente";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));
            cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
            cmd.Parameters.Add(new SqlParameter("@pApellido", pApellido));
            cmd.Parameters.Add(new SqlParameter("@pTelefono", pTelefono));
            cmd.Parameters.Add(new SqlParameter("@pCedula", pCedula));
            cmd.Parameters.Add(new SqlParameter("@clEmail", clEmail));

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista");
        }

        // POST: /Cliente/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int idCliente)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Mod_EstadoCl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));

            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista");
        }
    }
}
