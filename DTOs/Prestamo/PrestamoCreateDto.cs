namespace MiPrimeraAPI.DTOs.Prestamo
{
    public class PrestamoCreateDto
    {
        public int ClienteId { get; set; }
        public List<PrestamoDetalleCreateDto> Detalles { get; set; } = new();
    }
}
