using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.DTOs.Cliente;

public class ClienteResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public int MaxPrestamosSiultaneos { get; set; } = 3;
    public DateTime FechaRegistro { get; set; } 
    public TipoCliente Tipo { get; set; } = TipoCliente.Regular;

}
