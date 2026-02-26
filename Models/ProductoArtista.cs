namespace MiPrimeraAPI.Models
{
    public class ProductoArtista
    {
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public int ArtistaId { get; set; }
        public Artista Artista { get; set; } = null!;  

        public string Rol { get; set; } = string.Empty; 
    }
}
