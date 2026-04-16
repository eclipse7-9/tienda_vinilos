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
            var windowStart = now.AddMinutes(-30); // Ver últimas 10 bolas (30 min)

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

            if (!groupedStats.Any())
            {
                var currentBucket = (now.Minute / 3) * 3;
                groupedStats.Add(new { 
                    time = $"{now.Hour:D2}:{currentBucket:D2}:00", 
                    latency = 0.0,
                    totalDuration = 0.0,
                    queryCount = 0
                });
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
            
            // Intervalo de 3 minutos exactos desde el inicio de la bola
            var startTime = targetTime;
            var endTime = targetTime.AddMinutes(3);

            var logs = await _context.SqlLogs
                .Where(l => l.ExecutedAt >= startTime && l.ExecutedAt < endTime)
                .OrderByDescending(l => l.ExecutedAt)
                .Select(l => new {
                    l.Id,
                    l.CommandText,
                    l.DurationMs,
                    l.ExecutedAt,
                    Resources = $"{l.DurationMs}ms", // Recurso por consulta
                    IntervalResources = _context.SqlLogs // Recurso total intervalo (redundante pero seguro para el front)
                        .Where(x => x.ExecutedAt >= startTime && x.ExecutedAt < endTime)
                        .Sum(x => x.DurationMs)
                })
                .ToListAsync();

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error fetching SQL logs", details = ex.Message });
        }
    }
}