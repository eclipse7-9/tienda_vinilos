using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Auth;

public class RegisterDto
{
    [Required(ErrorMessage = "El email no puede estar vacío")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "La contraseña no puede estar vacía")]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[\W_]).{6,}$",
 ErrorMessage = "La contraseña debe tener al menos 6 caracteres, un número y un carácter especial.")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "El nombre no puede estar vacío")]
    [MaxLength(60)]
    public string Nombre { get; set; } = string.Empty;
    [MaxLength(60)]
    [Required(ErrorMessage = "El Apellido no puede estar vacío")]
    public string Apellido { get; set; } = string.Empty;
    [MaxLength(20)]
    [Required(ErrorMessage = "El teléfono no puede estar vacío")]
    public string Telefono { get; set; } = string.Empty;
}