using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace syswb_Taller;

public static class DatabaseInitializer
{
    public static string NormalizeConnectionString(string rawConnectionString)
    {
        if (string.IsNullOrWhiteSpace(rawConnectionString)) return rawConnectionString;

        try
        {
            var csBuilder = new SqlConnectionStringBuilder(rawConnectionString);

            // 1. Evitar bloqueos TLS en Linux y certificados autofirmados
            csBuilder.Encrypt = false;
            csBuilder.TrustServerCertificate = true;

            // 2. Conectar a todas las IPs en paralelo para evitar timeout de 30s
            csBuilder.MultiSubnetFailover = true;

            // 3. Priorizar IPv4 si la versión del driver lo permite
            try { csBuilder["IP Address Preference"] = "IPv4First"; } catch { }

            // 4. Si el host es un dominio interno de Railway (.railway.internal o server), resolver su IPv4 directamente
            var dataSource = csBuilder.DataSource ?? string.Empty;
            var parts = dataSource.Split(',');
            var host = parts[0].Trim();
            var port = parts.Length > 1 ? parts[1].Trim() : "1433";

            if (host.EndsWith(".railway.internal", StringComparison.OrdinalIgnoreCase) || host.Equals("server", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var addresses = Dns.GetHostAddresses(host);
                    var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                    if (ipv4 != null)
                    {
                        csBuilder.DataSource = $"{ipv4},{port}";
                    }
                }
                catch { }
            }

            return csBuilder.ConnectionString;
        }
        catch
        {
            return rawConnectionString;
        }
    }

    public static void Initialize(IServiceProvider services, IConfiguration configuration, string contentRootPath)
    {
        var logger = services.GetService<ILoggerFactory>()?.CreateLogger("DatabaseInitializer");
        var rawConn = configuration.GetConnectionString("ConexionSQL");

        if (string.IsNullOrWhiteSpace(rawConn))
        {
            logger?.LogWarning("No se encontró la cadena de conexión 'ConexionSQL'. Se omite inicialización automática.");
            return;
        }

        var connectionString = NormalizeConnectionString(rawConn);

        try
        {
            var csBuilder = new SqlConnectionStringBuilder(connectionString);
            var targetDatabase = string.IsNullOrWhiteSpace(csBuilder.InitialCatalog) ? "BD_Taller" : csBuilder.InitialCatalog;

            // Paso 1: Conectar a 'master' con reintentos para asegurar que la base de datos exista
            csBuilder.InitialCatalog = "master";
            csBuilder.ConnectTimeout = 15;

            logger?.LogInformation("Verificando existencia de la base de datos '{TargetDb}' en el servidor...", targetDatabase);

            bool connected = false;
            for (int attempt = 1; attempt <= 6; attempt++)
            {
                try
                {
                    using (var masterConn = new SqlConnection(csBuilder.ConnectionString))
                    {
                        masterConn.Open();
                        using (var cmd = masterConn.CreateCommand())
                        {
                            cmd.CommandText = $@"
                                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{targetDatabase}')
                                BEGIN
                                    CREATE DATABASE [{targetDatabase}];
                                END";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    connected = true;
                    logger?.LogInformation("Base de datos '{TargetDb}' verificada/creada exitosamente.", targetDatabase);
                    break;
                }
                catch (Exception ex)
                {
                    logger?.LogWarning("Intento {Attempt}/6 para conectar al motor SQL Server: {Message}", attempt, ex.Message);
                    Thread.Sleep(3000);
                }
            }

            if (!connected)
            {
                logger?.LogError("No se pudo conectar a SQL Server tras varios intentos. Se continuará con el inicio de la app.");
                return;
            }

            // Paso 2: Conectar a la base de datos destino y verificar si ya tiene tablas
            csBuilder.InitialCatalog = targetDatabase;
            using (var dbConn = new SqlConnection(csBuilder.ConnectionString))
            {
                dbConn.Open();

                bool tablesExist = false;
                using (var checkCmd = dbConn.CreateCommand())
                {
                    checkCmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name IN ('Empleado', 'Empleados', 'Roles')";
                    var count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    tablesExist = count > 0;
                }

                if (tablesExist)
                {
                    logger?.LogInformation("La base de datos '{TargetDb}' ya cuenta con tablas y registros. Inicialización completada.", targetDatabase);
                    return;
                }

                // Paso 3: Ejecutar el script SQL de inicialización
                var sqlFilePath = Path.Combine(contentRootPath, "BD_Taller_init.sql");
                if (!File.Exists(sqlFilePath))
                {
                    logger?.LogWarning("No se encontró el archivo '{SqlFilePath}' para inicializar las tablas.", sqlFilePath);
                    return;
                }

                logger?.LogInformation("Cargando estructura y registros desde BD_Taller_init.sql...");
                var sqlScript = File.ReadAllText(sqlFilePath);

                // Separar comandos por bloques GO
                var batches = Regex.Split(sqlScript, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                int executed = 0;
                foreach (var batch in batches)
                {
                    var trimmed = batch.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed)) continue;

                    // Omitir comandos redundantes de USE master o ALTER DATABASE complejos
                    if (trimmed.StartsWith("USE [master]", StringComparison.OrdinalIgnoreCase)) continue;
                    if (trimmed.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)) continue;

                    try
                    {
                        using (var cmd = dbConn.CreateCommand())
                        {
                            cmd.CommandText = trimmed;
                            cmd.CommandTimeout = 120;
                            cmd.ExecuteNonQuery();
                        }
                        executed++;
                    }
                    catch (Exception ex)
                    {
                        // Si falla algún comando no crítico (como fulltext o alter compatibility), registrar advertencia y continuar
                        logger?.LogDebug("Aviso en bloque SQL: {Message}", ex.Message);
                    }
                }

                logger?.LogInformation("Inicialización finalizada con éxito. Se ejecutaron {Count} bloques SQL correctamente.", executed);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Excepción durante la inicialización de la base de datos.");
        }
    }
}
