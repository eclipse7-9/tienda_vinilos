namespace MiPrimeraAPI.Models
{
    public class PrestamoDetalle
    {
         public int Id { get; set; }
         public string? Observaciones { get; set; }

        public int PrestamoId { get; set; } 
        public Prestamo Prestamo { get; set; } = null!;

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
