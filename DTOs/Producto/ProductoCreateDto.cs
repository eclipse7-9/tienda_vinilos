using System.ComponentModel.DataAnnotations;

namespace MiPrimeraAPI.DTOs.Producto
{
    public class ProductoCreateDto
    {
        [MaxLength(120)]
        [Required(ErrorMessage = "El título del producto no puede estar vacío")]
        public string Titulo { get; set; } = string.Empty;
        [Required(ErrorMessage = "La descripción del producto no puede estar vacío")]
        [MaxLength(1120)]
        public string Descripcion { get; set; } = string.Empty;
        [Required(ErrorMessage = "El precio del producto no puede estar vacío")]
        [Range(0.01, 99999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }
        [Range(1900, 2100, ErrorMessage = "Año no válido")]
        [Required(ErrorMessage = "El año de lanzamiento del producto no puede estar vacío")]
        public int AnioLanzamiento { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;

        public int CategoriaId { get; set; }
        public List<int> ArtistaIds { get; set; } = new();
        public int StockInicial { get; set; } = 0;
        public int StockDisponible { get; set; } = 0;
        public int StockDisponiblePrestamo { get; set; } = 0;
    }
}
