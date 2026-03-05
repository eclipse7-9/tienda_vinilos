namespace MiPrimeraAPI.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public TipoCliente Tipo { get; set; } = TipoCliente.Regular;
        public int MaxPrestamosSiultaneos { get; set; } = 3;

        //relación con préstamos y ventas
        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        //relación con usuario
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
