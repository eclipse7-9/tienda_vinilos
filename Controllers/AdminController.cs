using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public AdminController(MediaStoreContext context) => _context = context;

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalProductos = await _context.Productos.CountAsync();
        var totalClientes = await _context.Clientes.CountAsync();
        var totalVentas = await _context.Ventas.CountAsync();
        var totalPrestamos = await _context.Prestamos.CountAsync();
        var ingresoTotal = await _context.Ventas.SumAsync(v => v.Total);
        var prestamosActivos = await _context.Prestamos
            .CountAsync(p => p.Estado == 0);
        var productosConPocoStock = await _context.Inventarios
            .CountAsync(i => i.StockDisponible <= i.StockMinimo);
        var ventasRecientes = await _context.Ventas
            .Include(v => v.Cliente)
            .OrderByDescending(v => v.Fecha)
            .Take(5)
            .Select(v => new {
                v.Id,
                v.Fecha,
                v.Total,
                v.MetodoPago,
                v.Estado,
                Cliente = v.Cliente.Nombre + " " + v.Cliente.Apellido
            })
            .ToListAsync();

        return Ok(new
        {
            totalProductos,
            totalClientes,
            totalVentas,
            totalPrestamos,
            ingresoTotal,
            prestamosActivos,
            productosConPocoStock,
            ventasRecientes,
        });
    }

    /** [Authorize(Roles = "Admin")] **/
    [AllowAnonymous]
    [HttpGet("db-stats")]
    public async Task<IActionResult> GetDbStats()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Ejecutamos una consulta simple para medir latencia real
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            stopwatch.Stop();
            var latency = stopwatch.ElapsedMilliseconds;

            // Generamos datos para la gráfica de Recharts usando la latencia real
            var stats = new List<object>
            {
                new { time = DateTime.Now.AddSeconds(-50).ToString("HH:mm:ss"), latency = Math.Max(5, latency + new Random().Next(-5, 5)) },
                new { time = DateTime.Now.AddSeconds(-40).ToString("HH:mm:ss"), latency = Math.Max(5, latency + new Random().Next(-5, 5)) },
                new { time = DateTime.Now.AddSeconds(-30).ToString("HH:mm:ss"), latency = Math.Max(5, latency + new Random().Next(-5, 5)) },
                new { time = DateTime.Now.AddSeconds(-20).ToString("HH:mm:ss"), latency = Math.Max(5, latency + new Random().Next(-5, 5)) },
                new { time = DateTime.Now.AddSeconds(-10).ToString("HH:mm:ss"), latency = Math.Max(5, latency + new Random().Next(-5, 5)) },
                new { time = DateTime.Now.ToString("HH:mm:ss"), latency = latency }
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "DB connection error", details = ex.Message });
        }
    }
}