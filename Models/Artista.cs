namespace MiPrimeraAPI.Models
{
    public class Artista
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Biografia { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;

        public ICollection<ProductoArtista> ProductoArtistas { get; set; } = new List<ProductoArtista>();
    }
}
