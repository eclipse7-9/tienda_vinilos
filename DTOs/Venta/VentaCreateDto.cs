namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaCreateDto
    {
        public int ClienteId { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public List<VentaDetalleCreateDto> Detalles { get; set; } = new();

    }
}
