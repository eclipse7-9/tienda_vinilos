using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Models;
using Microsoft.EntityFrameworkCore;

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
            
            // 1. Intentar obtener de Redis
            var vinilosCached = await _cache.GetStringAsync(cacheKey);

            if (vinilosCached != null)
            {
                var vinilos = JsonSerializer.Deserialize<List<Producto>>(vinilosCached);
                return Ok(vinilos);
            }

            // 2. Si no está en Redis, vamos a la DB
            var vinilosDb = await _context.Productos.ToListAsync();

            // 3. Guardar en Redis (expira en 10 min)
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            var serializedVinilos = JsonSerializer.Serialize(vinilosDb);
            await _cache.SetStringAsync(cacheKey, serializedVinilos, options);

            return Ok(vinilosDb);
        }
    }
}