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

    [Authorize(Roles = "Admin")]
    [HttpGet("health-metrics")]
public IActionResult GetTechnicalMetrics()
{
    // Aquí podrías integrar los datos que OpenTelemetry está recolectando
    return Ok(new {
        dbStatus = "Online",
        lastSlowQueryMs = 450, // Dato que vendría de tu Interceptor
        activeConnections = 12
    });
}
}