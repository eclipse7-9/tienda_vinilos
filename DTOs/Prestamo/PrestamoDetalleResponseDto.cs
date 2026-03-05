namespace MiPrimeraAPI.DTOs.Prestamo
{
    public class PrestamoDetalleResponseDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string TituloProducto { get; set; } = string.Empty;
        public string? Observaciones { get; set; }


    }
}
