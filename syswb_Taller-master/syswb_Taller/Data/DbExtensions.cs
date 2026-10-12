using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace syswb_Taller.Data;

/// <summary>
/// Atajos para ejecutar procedimientos almacenados con ADO.NET sobre la conexión del DbContext.
/// Los valores null se envían como DBNull.
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Abre la conexión del contexto (si hace falta) y crea un comando para el procedimiento indicado.
    /// La conexión la administra EF Core, por eso no se debe liberar manualmente.
    /// </summary>
    public static async Task<DbCommand> CreateStoredProcedureAsync(
        this DatabaseFacade database,
        string procedureName,
        params (string Name, object? Value)[] parameters)
    {
        var connection = database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        return connection.CreateStoredProcedure(procedureName, null, parameters);
    }

    /// <summary>
    /// Abre la conexión del contexto (si hace falta) y crea un comando de texto SQL parametrizado.
    /// </summary>
    public static async Task<DbCommand> CreateSqlCommandAsync(
        this DatabaseFacade database,
        string sql,
        params (string Name, object? Value)[] parameters)
    {
        var connection = database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;

        foreach (var (name, value) in parameters)
            command.AddParameter(name, value);

        return command;
    }

    /// <summary>Crea un comando para un procedimiento almacenado sobre una conexión ya abierta.</summary>
    public static DbCommand CreateStoredProcedure(
        this DbConnection connection,
        string procedureName,
        DbTransaction? transaction = null,
        params (string Name, object? Value)[] parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = procedureName;
        command.CommandType = CommandType.StoredProcedure;
        command.Transaction = transaction;

        foreach (var (name, value) in parameters)
            command.AddParameter(name, value);

        return command;
    }

    /// <summary>Ejecuta un procedimiento almacenado que no devuelve filas.</summary>
    public static async Task<int> ExecuteStoredProcedureAsync(
        this DatabaseFacade database,
        string procedureName,
        params (string Name, object? Value)[] parameters)
    {
        await using var command = await database.CreateStoredProcedureAsync(procedureName, parameters);
        return await command.ExecuteNonQueryAsync();
    }

    public static DbCommand AddParameter(this DbCommand command, string name, object? value)
    {
        command.Parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
        return command;
    }

    public static SqlParameter AddOutputParameter(this DbCommand command, string name, SqlDbType type)
    {
        var parameter = new SqlParameter(name, type) { Direction = ParameterDirection.Output };
        command.Parameters.Add(parameter);
        return parameter;
    }

    public static string? GetNullableString(this DbDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    public static int? GetNullableInt32(this DbDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);

    public static double? GetNullableDouble(this DbDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);

    public static DateTime? GetNullableDateTime(this DbDataReader reader, int ordinal) =>
        reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
}
