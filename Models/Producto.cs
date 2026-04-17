namespace MiPrimeraAPI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int AnioLanzamiento { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;

        // Tipo: Vinilo, CD, Libro, DVD
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        // Relación N:M con Artistas/Autores
        public ICollection<ProductoArtista> ProductoArtistas { get; set; } = new List<ProductoArtista>();

        //relación 1:1 con Inventario
        public Inventario Inventario { get; set; } = null!;
    }
}