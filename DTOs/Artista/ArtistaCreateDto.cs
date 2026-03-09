using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Artista
{
    public class ArtistaCreateDto
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;
        //
        [MaxLength(1000)]
        public string Biografia { get; set; } = string.Empty;
        //
        [Required(ErrorMessage = "El tipo es obligatorio")]
        public string Tipo { get; set; } = string.Empty;
    }
}
