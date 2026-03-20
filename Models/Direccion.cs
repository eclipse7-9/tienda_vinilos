using Microsoft.Identity.Client;

namespace MiPrimeraAPI.Models
{
    public class Direccion
    {
       public int Id { get; set; }
       public string Nombre { get; set; } = string.Empty;
       public string Calle { get; set; } = string.Empty;
       public string Ciudad { get; set; } = string.Empty;
       public string Departamento { get; set; } = string.Empty;
       public string CodigoPostal { get; set; } = string.Empty;
       public string? Referencia { get; set; }
       public bool EsPrincipal { get; set; }    
       public int ClienteId { get; set; }
       public Cliente Cliente { get; set; } = null!;
    }
}
