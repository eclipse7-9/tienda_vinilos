namespace MiPrimeraAPI.DTOs.Prestamo
{
    public class PrestamoResponseDto
    {
       public int Id { get; set; }  
       public DateTime FechaPrestamo { get; set; }
       public DateTime FechaDevolucionEsperada { get; set; }
       public DateTime? FechaDevolucionReal { get; set; }
       public string Estado { get; set; } = string.Empty;
       public decimal? Multa { get; set; }
       
       public int ClienteId { get; set; }
       public List<PrestamoDetalleResponseDto> Detalles { get; set; } = new();
    }
}
