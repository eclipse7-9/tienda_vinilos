namespace MiPrimeraAPI.DTOs.Venta
{
    public class VentaDetalleResponseDto
    {
       public int Cantidad { get; set; }
       public string TituloProducto { get; set; } = string.Empty;
       public decimal PrecioUnitario { get; set; }
       public decimal Subtotal { get; set; }
        
       public int ProductoId { get; set; }
    }
}
