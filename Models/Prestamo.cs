namespace MiPrimeraAPI.Models
{
    public enum EstadoPrestamo
    {
        Activo,
        Devuelto,
        Vencido,
        PerdidoODañado

    }
    public class Prestamo
    { 
    
        public int Id { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public EstadoPrestamo Estado { get; set; }
        public decimal? Multa { get; set; }

        //Relación con Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        //Relación con Detalles del Préstamo
        public ICollection<PrestamoDetalle> Detalles { get; set; } = new List<PrestamoDetalle>();


    }
}
