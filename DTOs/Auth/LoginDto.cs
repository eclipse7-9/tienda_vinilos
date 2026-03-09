using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "El campo no puede estar vacío")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "El campo no puede estar vacío")]
    public string Password { get; set; } = string.Empty;
}