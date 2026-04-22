using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Empleado")]
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

    [AllowAnonymous]
    [HttpGet("db-stats")]
    public async Task<IActionResult> GetDbStats()
    {
        try
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-60); // Ampliamos a 60 min para mayor visibilidad

            var stats = await _context.SqlLogs
                .Where(l => l.ExecutedAt > windowStart)
                .ToListAsync();

            var groupedStats = stats
                .GroupBy(l => {
                    var minutes = (l.ExecutedAt.Minute / 3) * 3;
                    return new DateTime(l.ExecutedAt.Year, l.ExecutedAt.Month, l.ExecutedAt.Day, 
                                     l.ExecutedAt.Hour, minutes, 0, DateTimeKind.Utc);
                })
                .Select(g => new {
                    time = g.Key.ToString("HH:mm:ss"),
                    latency = Math.Round(g.Average(l => l.DurationMs), 2),
                    totalDuration = Math.Round(g.Sum(l => l.DurationMs), 2),
                    queryCount = g.Count()
                })
                .OrderBy(x => x.time)
                .ToList();

            // Si no hay nada, devolvemos al menos un punto para que se vea algo en la gráfica
            if (!groupedStats.Any())
            {
                for (int i = 0; i < 5; i++)
                {
                    var t = now.AddMinutes(-i * 3);
                    var bucket = (t.Minute / 3) * 3;
                    groupedStats.Add(new { 
                        time = $"{(t.Hour):D2}:{bucket:D2}:00", 
                        latency = 0.0,
                        totalDuration = 0.0,
                        queryCount = 0
                    });
                }
                groupedStats = groupedStats.OrderBy(x => x.time).ToList();
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
            // Buscamos logs que coincidan con la hora/minuto, ignorando el día si es necesario para depuración
            // Pero intentamos primero con el día actual
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 
                int.Parse(parts[0]), int.Parse(parts[1]), 0, DateTimeKind.Utc);
            
            var startTime = targetTime.AddSeconds(-10); // Margen
            var endTime = targetTime.AddMinutes(4); // Margen

            var logs = await _context.SqlLogs
                .Where(l => l.ExecutedAt >= startTime && l.ExecutedAt <= endTime)
                .OrderByDescending(l => l.ExecutedAt)
                .Take(100)
                .Select(l => new {
                    l.Id,
                    l.CommandText,
                    l.DurationMs,
                    l.ExecutedAt
                })
                .ToListAsync();

            // Si no encuentra nada con el día actual, buscamos los últimos 100 logs globales
            if (!logs.Any())
            {
                logs = await _context.SqlLogs
                    .OrderByDescending(l => l.ExecutedAt)
                    .Take(20)
                    .Select(l => new {
                        l.Id,
                        l.CommandText,
                        l.DurationMs,
                        l.ExecutedAt
                    })
                    .ToListAsync();
            }

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error fetching SQL logs", details = ex.Message });
        }
    }
}
