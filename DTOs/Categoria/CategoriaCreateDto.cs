
using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Categoria;

public class CategoriaCreateDto
{
    [MaxLength(100)]
    [Required(ErrorMessage = "El nombre de la categoría no puede estar vacío")]
    public String Nombre { get; set; } = string.Empty;
    [MaxLength(1000)]
    [Required(ErrorMessage = "La descripción de la categoría no puede estar vacío")]
    public String Descripcion { get; set; } = string.Empty;
    public String Icono { get; set; } = string.Empty;
    public bool PermitePrestamo { get; set; }
}
