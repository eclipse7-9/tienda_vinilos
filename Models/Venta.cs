namespace MiPrimeraAPI.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public int? MetodoPagoId { get; set; }
        public int? DireccionId { get; set; }
        public string Estado { get; set; } = string.Empty;

        public Cliente Cliente { get; set; } = null!;
        public int ClienteId { get; set; }

        public MetodoPago? MetodoPagoRel { get; set; }
        public Direccion? Direccion { get; set; }

        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();


    }
}
