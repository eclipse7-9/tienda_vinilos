using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaCreateDto
    {
        public int ClienteId { get; set; }
        [Required(ErrorMessage = "Debes prorporcionar un método de pago")]
        public string MetodoPago { get; set; } = string.Empty;
        [MinLength(1, ErrorMessage = "Debes agregar al menos un producto")]
        public List<VentaDetalleCreateDto> Detalles { get; set; } = new();

    }
}
