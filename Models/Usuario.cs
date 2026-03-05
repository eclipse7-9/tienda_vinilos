namespace MiPrimeraAPI.Models;

public class Usuario
{
    public int Id { get; set; }

    // El email es único y obligatorio
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;

    public Cliente? Cliente { get; set; }
}