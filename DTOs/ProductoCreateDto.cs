namespace MiPrimeraAPI.DTOs.Producto
{
    public class ProductoCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int AnioLanzamiento { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;

        public int CategoriaId { get; set; }
        public List<int> ArtistaIds { get; set; } = new();
        public int StockInicial { get; set; } = 0;
    }
}
