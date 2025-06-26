using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    [Route("Cliente")]  // Ruta base para todas las acciones
    public class ClienteController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public ClienteController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        // GET: /Cliente/Lista
        [HttpGet("Lista")]
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
        [HttpGet("Crear")]
        public IActionResult Crear(string redirigirA = null)
        {
            ViewBag.Redireccion = redirigirA;
            return View();
        }


        // POST: /Cliente/Crear
        [HttpPost("Crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            string pNombre,
            string pApellido,
            string pTelefono,
            string pCedula,
            string clEmail,
            string redirigirA = null)
        {
            pTelefono = pTelefono.Replace("-", "");
            int idCliente = 0;

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

            var outParam = new SqlParameter("@OutIdCliente", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();
            idCliente = (int)outParam.Value;

            // Redirección condicional
            if (!string.IsNullOrEmpty(redirigirA))
            {
                return RedirectToAction("Crear", "Vehiculo", new { idCliente = idCliente, redirigirA = redirigirA });
            }

            return RedirectToAction("Lista");
        }


        // GET: /Cliente/Editar/5
        [HttpGet("Editar/{idCliente}")]
        public async Task<IActionResult> Editar(int idCliente)
        {
            dynamic cliente = null;
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

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
         WHERE c.idCliente = @idCliente";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    cliente = new
                    {
                        IdCliente = reader.GetInt32(0),
                        Nombre = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Apellido = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Telefono = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Cedula = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Email = reader.IsDBNull(5) ? null : reader.GetString(5)
                    };
                }
            }

            if (cliente == null)
                return NotFound();

            ViewBag.Cliente = cliente;

            var vehiculos = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_VehiculosPorCliente";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    vehiculos.Add(new
                    {
                        Matricula = reader.IsDBNull(0) ? null : reader.GetString(0),
                        Color = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Marca = reader.IsDBNull(2) ? null : reader.GetString(2)
                    });
                }
            }
            ViewBag.Vehiculos = vehiculos;

            var ordenes = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_OrdenesPorCliente";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));
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
                        Estado = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Tipo = reader.IsDBNull(3) ? null : reader.GetString(3),
                        FechaEntrega = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                        FechaFin = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5),
                        CostoTotal = reader.GetDouble(6),
                        VehiculoMatric = reader.IsDBNull(7) ? null : reader.GetString(7),
                        VehiculoMarca = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
            ViewBag.Ordenes = ordenes;

            return View();
        }

        // POST: /Cliente/Editar/5
        [HttpPost("Editar/{idCliente}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int idCliente,
            string pNombre,
            string pApellido,
            string pTelefono,
            string pCedula,
            string clEmail)
        {
            pTelefono = pTelefono.Replace("-", "");

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

        // POST: /Cliente/CambiarEstado/5
        [HttpPost("CambiarEstado/{idCliente}")]
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
