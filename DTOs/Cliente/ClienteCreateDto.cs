using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Cliente
{
    public class ClienteCreateDto
    {
        [MaxLength(60)]
        [Required(ErrorMessage = "Debes proporcionar al menos un nombre")]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(60)]
        [Required(ErrorMessage = "Debes proporcionar al menos un apellido")]
        public string Apellido { get; set; } = string.Empty;
        [MaxLength(60)]
        [Required(ErrorMessage = "Debes proporcionar un email")]
        public string Email { get; set; } = string.Empty;
        [MaxLength(60)]
        [Required(ErrorMessage = "Debes proporcionar un teléfono")]
        public string Telefono { get; set; } = string.Empty;
    }
}
