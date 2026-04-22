using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Models;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.DTOs.Producto;

namespace MiPrimeraAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VinilosController : ControllerBase
    {
        private readonly MediaStoreContext _context;
        private readonly IDistributedCache _cache;

        public VinilosController(MediaStoreContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string cacheKey = "lista_vinilos";
            
            try 
            {
                // 1. Intentar obtener de Redis
                var vinilosCached = await _cache.GetStringAsync(cacheKey);

                if (vinilosCached != null)
                {
                    var vinilos = JsonSerializer.Deserialize<List<ProductoResponseDto>>(vinilosCached);
                    return Ok(vinilos);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] No se pudo leer de la cache: {ex.Message}");
                // Continuamos a la DB si falla Redis
            }

            // 2. Si no está en Redis, vamos a la DB
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Inventario)
                .Include(p => p.ProductoArtistas)
                    .ThenInclude(pa => pa.Artista)
                .ToListAsync();

            var response = productos.Select(p => new ProductoResponseDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                AnioLanzamiento = p.AnioLanzamiento,
                ImagenUrl = p.ImagenUrl,
                CategoriaId = p.CategoriaId,
                EstaActivo = p.EstaActivo,
                Categoria = p.Categoria == null ? null : new CategoriaDetalleDto
                {
                    Nombre = p.Categoria.Nombre,
                    PermitePrestamo = p.Categoria.PermitePrestamo
                },
                Inventario = p.Inventario == null ? null : new InventarioDto
                {
                    StockDisponible = p.Inventario.StockDisponible,
                    StockDisponiblePrestamo = p.Inventario.StockDisponiblePrestamo
                },
                Artistas = p.ProductoArtistas.Select(pa => new ArtistaDto
                {
                    Nombre = pa.Artista?.Nombre ?? "Desconocido"
                }).ToList()
            }).ToList();

            try
            {
                // 3. Guardar en Redis (expira en 10 min)
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                var serializedVinilos = JsonSerializer.Serialize(response);
                await _cache.SetStringAsync(cacheKey, serializedVinilos, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis Error] No se pudo guardar en la cache: {ex.Message}");
            }

            return Ok(response);
        }
    }
}
