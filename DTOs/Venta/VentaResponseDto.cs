namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public int ClienteId { get; set; }
        public List<VentaDetalleResponseDto> Detalles { get; set; } = new();

    }
}
