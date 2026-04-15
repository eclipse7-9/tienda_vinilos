using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Data
{
    public class MediaStoreContext : DbContext
    {
        public MediaStoreContext(DbContextOptions<MediaStoreContext> Options)
            : base(Options) { }

        // DbSet para cada entidad
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<PrestamoDetalle> PrestamoDetalles { get; set; }
        public DbSet<ProductoArtista> ProductoArtistas { get; set; }
        public DbSet<VentaDetalle> VentasDetalle { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<MetodoPago> MetodoPago { get; set; }
        public DbSet<SqlLog> SqlLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductoArtista>()
                .HasKey(pa => new { pa.ProductoId, pa.ArtistaId });


            // Precisión para los decimales

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaDetalle>()
                .Property(vd => vd.PrecioUnitario)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VentaDetalle>()
                .Property(vd => vd.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.Multa)
                .HasPrecision(18, 2);
  }
 }
}
