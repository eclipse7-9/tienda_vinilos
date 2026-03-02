namespace MiPrimeraAPI.DTOs.Producto
{
    public class ProductoResponseDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int AnioLanzamiento { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
    }
}
