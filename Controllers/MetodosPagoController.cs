using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MetodosPagoController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public MetodosPagoController(MediaStoreContext context) => _context = context;

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
    {
        var metodos = await _context.MetodoPago
            .Where(m => m.ClienteId == clienteId)
            .ToListAsync();
        return Ok(metodos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MetodoPago dto)
    {
        if (dto.EsPrincipal)
        {
            var otros = await _context.MetodoPago
                .Where(m => m.ClienteId == dto.ClienteId)
                .ToListAsync();
            otros.ForEach(m => m.EsPrincipal = false);
        }
        _context.MetodoPago.Add(dto);
        await _context.SaveChangesAsync();
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, MetodoPago dto)
    {
        var metodo = await _context.MetodoPago.FindAsync(id);
        if (metodo == null) return NotFound();

        if (dto.EsPrincipal)
        {
            var otros = await _context.MetodoPago
                .Where(m => m.ClienteId == metodo.ClienteId && m.Id != id)
                .ToListAsync();
            otros.ForEach(m => m.EsPrincipal = false);
        }

        metodo.Tipo = dto.Tipo;
        metodo.NombreTitular = dto.NombreTitular;
        metodo.UltimosDigitos = dto.UltimosDigitos;
        metodo.Banco = dto.Banco;
        metodo.EsPrincipal = dto.EsPrincipal;

        await _context.SaveChangesAsync();
        return Ok(metodo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var metodo = await _context.MetodoPago.FindAsync(id);
        if (metodo == null) return NotFound();
        _context.MetodoPago.Remove(metodo);
        await _context.SaveChangesAsync();
        return Ok();
    }
}