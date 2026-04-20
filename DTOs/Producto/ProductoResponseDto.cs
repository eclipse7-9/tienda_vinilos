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

        public CategoriaDetalleDto? Categoria { get; set; }
        public InventarioDto? Inventario { get; set; }
        public List<ArtistaDto> Artistas { get; set; } = new();
    }

    public class CategoriaDetalleDto {
        public string Nombre { get; set; } = string.Empty;
        public bool PermitePrestamo { get; set; }
    }

    public class InventarioDto {
        public int StockDisponible { get; set; }
        public int StockDisponiblePrestamo { get; set; }
    }

    public class ArtistaDto {
        public string Nombre { get; set; } = string.Empty;
    }
}