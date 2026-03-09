using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaDetalleCreateDto
    {
        [Range(1, 999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
        
        public int ProductoId { get; set; }
    }
}
