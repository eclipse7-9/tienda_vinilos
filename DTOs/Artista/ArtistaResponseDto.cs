using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Artista
{
    public class ArtistaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Biografia { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
