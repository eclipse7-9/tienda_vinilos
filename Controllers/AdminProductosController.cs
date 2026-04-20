using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Producto;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Empleado")]
public class AdminProductosController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public AdminProductosController(MediaStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
    {
        var productos = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Inventario)
            .Include(p => p.ProductoArtistas)
                .ThenInclude(pa => pa.Artista)
            .OrderByDescending(p => p.Id)
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
        });

        return Ok(response);
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        producto.EstaActivo = !producto.EstaActivo;
        await _context.SaveChangesAsync();

        return Ok(new { id = producto.Id, estaActivo = producto.EstaActivo });
    }
}
