namespace MiPrimeraAPI.DTOs.Categoria
{
    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public bool PermitePrestamo { get; set; }
    }
}
