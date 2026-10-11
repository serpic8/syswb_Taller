namespace syswb_Taller.Data;

/// <summary>
/// Nombres de rol usados en los atributos [Authorize]. Deben coincidir con Roles.RTipo en la base de datos.
/// </summary>
public static class RolesSistema
{
    public const string Administrador = "Administrador";
    public const string Cajero = "Cajero";
    public const string Supervisor = "Supervisor";
    public const string Mecanico = "Mecanico";

    public const string AdminSupervisor = Administrador + "," + Supervisor;
    public const string Gestion = Administrador + "," + Cajero + "," + Supervisor;
    public const string Todos = Gestion + "," + Mecanico;
}
