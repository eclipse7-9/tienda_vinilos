using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Models;
using MiPrimeraAPI.DTOs.Direccion;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DireccionesController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public DireccionesController(MediaStoreContext context) => _context = context;

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var direcciones = await _context.Direcciones
            .Where(d => d.ClienteId == clienteId)
            .ToListAsync();
        return Ok(direcciones);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DireccionCreateDto dto)
    {
        var direccion = new Direccion
        {
            Nombre = dto.Nombre,
            Calle = dto.Calle,
            Ciudad = dto.Ciudad,
            Departamento = dto.Departamento,
            CodigoPostal = dto.CodigoPostal,
            Referencia = dto.Referencia,
            EsPrincipal = dto.EsPrincipal,
            ClienteId = dto.ClienteId,
        };

        if (dto.EsPrincipal)
        {
            var otras = await _context.Direcciones
                .Where(d => d.ClienteId == dto.ClienteId)
                .ToListAsync();
            otras.ForEach(d => d.EsPrincipal = false);
        }

        _context.Direcciones.Add(direccion);
        await _context.SaveChangesAsync();
        return Ok(direccion);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DireccionCreateDto dto)
    {
        var direccion = await _context.Direcciones.FindAsync(id);
        if (direccion == null) return NotFound();

        if (dto.EsPrincipal)
        {
            var otras = await _context.Direcciones
                .Where(d => d.ClienteId == direccion.ClienteId && d.Id != id)
                .ToListAsync();
            otras.ForEach(d => d.EsPrincipal = false);
        }

        direccion.Nombre = dto.Nombre;
        direccion.Calle = dto.Calle;
        direccion.Ciudad = dto.Ciudad;
        direccion.Departamento = dto.Departamento;
        direccion.CodigoPostal = dto.CodigoPostal;
        direccion.Referencia = dto.Referencia;
        direccion.EsPrincipal = dto.EsPrincipal;

        await _context.SaveChangesAsync();
        return Ok(direccion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var direccion = await _context.Direcciones.FindAsync(id);
        if (direccion == null) return NotFound();
        _context.Direcciones.Remove(direccion);
        await _context.SaveChangesAsync();
        return Ok();
    }
}