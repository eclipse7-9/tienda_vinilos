namespace MiPrimeraAPI.Models
{
    public class Inventario
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public int StockTotal { get; set; }
        public int StockDisponible { get; set; }
        public int StockDisponiblePrestamo { get; set; }
        public int StockEnPrestamo { get; set; }
        public int StockMinimo  { get; set; }

    }
}
