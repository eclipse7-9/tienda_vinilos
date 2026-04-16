using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaCreateDto
    {
        public int ClienteId { get; set; }
        public int? MetodoPagoId { get; set; }
        public int? DireccionId { get; set; }
        public string? MetodoPago { get; set; }
        [MinLength(1, ErrorMessage = "Debes agregar al menos un producto")]
        public List<VentaDetalleCreateDto> Detalles { get; set; } = new();

    }
}
