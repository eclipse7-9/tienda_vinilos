using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public NotificacionesController(MediaStoreContext context) => _context = context;

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var notifications = await _context.Notificaciones
            .Where(n => n.ClienteId == clienteId)
            .OrderByDescending(n => n.Fecha)
            .Take(20)
            .ToListAsync();
        return Ok(notifications);
    }

    [HttpPatch("{id}/leer")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notif = await _context.Notificaciones.FindAsync(id);
        if (notif == null) return NotFound();
        
        notif.Leida = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("cliente/{clienteId}")]
    public async Task<IActionResult> ClearAll(int clienteId)
    {
        var notifications = await _context.Notificaciones
            .Where(n => n.ClienteId == clienteId)
            .ToListAsync();
        
        _context.Notificaciones.RemoveRange(notifications);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
