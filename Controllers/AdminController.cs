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
        try
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-2); // Últimos 2 minutos

            var stats = await _context.SqlLogs
                .Where(l => l.ExecutedAt > windowStart)
                .ToListAsync();

            var groupedStats = stats
                .GroupBy(l => new { 
                    Hour = l.ExecutedAt.Hour, 
                    Minute = l.ExecutedAt.Minute, 
                    Second = (l.ExecutedAt.Second / 10) * 10 
                })
                .Select(g => new {
                    time = $"{g.Key.Hour:D2}:{g.Key.Minute:D2}:{g.Key.Second:D2}",
                    latency = Math.Round(g.Average(l => l.DurationMs), 2)
                })
                .OrderBy(x => x.time)
                .ToList();

            // Si no hay datos, devolvemos al menos el punto actual con 0
            if (!groupedStats.Any())
            {
                groupedStats.Add(new { time = now.ToString("HH:mm:ss"), latency = 0.0 });
            }

            return Ok(groupedStats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error fetching DB stats", details = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("sql-logs")]
    public async Task<IActionResult> GetSqlLogs([FromQuery] string time)
    {
        try
        {
            if (string.IsNullOrEmpty(time)) return BadRequest("Time is required");

            var parts = time.Split(':');
            if (parts.Length != 3) return BadRequest("Invalid time format (HH:mm:ss)");

            var now = DateTime.UtcNow;
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 
                int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), DateTimeKind.Utc);
            
            var startTime = targetTime.AddSeconds(-5);
            var endTime = targetTime.AddSeconds(15);

            var logs = await _context.SqlLogs
                .Where(l => l.ExecutedAt >= startTime && l.ExecutedAt <= endTime)
                .OrderByDescending(l => l.ExecutedAt)
                .Take(50)
                .ToListAsync();

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error fetching SQL logs", details = ex.Message });
        }
    }
}