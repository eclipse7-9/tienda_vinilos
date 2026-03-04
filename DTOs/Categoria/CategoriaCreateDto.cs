
namespace MiPrimeraAPI.DTOs.Categoria;

public class CategoriaCreateDto
{
    public String Nombre { get; set; } = string.Empty;
    public String Descripcion { get; set; } = string.Empty;
    public String Icono { get; set; } = string.Empty;
    public bool PermitePrestamo { get; set; }
}
