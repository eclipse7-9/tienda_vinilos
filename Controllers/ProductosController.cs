using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Producto;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public ProductosController(MediaStoreContext Context)
    {
        _context = Context;
    }

    [HttpGet]

    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
    {
        var productos = await _context.Productos.ToListAsync();
        var response = productos.Select(C => new ProductoResponseDto
        {
            Id = C.Id,
            Titulo = C.Titulo,
            Descripcion = C.Descripcion,
            Precio = C.Precio,
            AnioLanzamiento = C.AnioLanzamiento,
            ImagenUrl = C.ImagenUrl,
            CategoriaId = C.CategoriaId
        });

        return Ok(response);
    }

    [HttpGet("{id}")]

    public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null) return NotFound();

        return Ok(new ProductoResponseDto
        {
            Id = producto.Id,
            Titulo = producto.Titulo,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            AnioLanzamiento = producto.AnioLanzamiento,
            ImagenUrl = producto.ImagenUrl,
            CategoriaId = producto.CategoriaId
        });

    }


    [Authorize(Roles = "Admin, Empleado")]
    [HttpPost]
    public async Task<ActionResult<ProductoResponseDto>> Create(ProductoCreateDto dto)
    {
        var producto = new Producto
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            AnioLanzamiento = dto.AnioLanzamiento,
            ImagenUrl = dto.ImagenUrl,
            CategoriaId = dto.CategoriaId,
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var inventario = new Inventario
        {
            ProductoId = producto.Id,
            StockTotal = dto.StockInicial,
            StockDisponible = dto.StockInicial,
            StockDisponiblePrestamo = dto.StockInicial,
            StockEnPrestamo = 0,
            StockMinimo = 5
        };
        _context.Inventarios.Add(inventario);

        var productoArtistas = dto.ArtistaIds.Select(artistaId => new ProductoArtista
        {
            ProductoId = producto.Id,
            ArtistaId = artistaId
        }).ToList();

        _context.ProductoArtistas.AddRange(productoArtistas);
        await _context.SaveChangesAsync();

        var response = new ProductoResponseDto
        {
            Id = producto.Id,
            Titulo = producto.Titulo,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            AnioLanzamiento = producto.AnioLanzamiento,
            ImagenUrl = producto.ImagenUrl,
            CategoriaId = producto.CategoriaId
        };

        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, response);
    }

    [Authorize(Roles = "Admin, Empleado")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductoCreateDto dto)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null) return NotFound();

        producto.Titulo = dto.Titulo;
        producto.Descripcion = dto.Descripcion;
        producto.Precio = dto.Precio;
        producto.AnioLanzamiento = dto.AnioLanzamiento;
        producto.ImagenUrl = dto.ImagenUrl;
        producto.CategoriaId = dto.CategoriaId;

        var artistasActuales = _context.ProductoArtistas
            .Where(pa => pa.ProductoId == id);
        _context.ProductoArtistas.RemoveRange(artistasActuales);

        var artistasNuevos = dto.ArtistaIds.Select(artistaId => new ProductoArtista
        {
            ProductoId = id,
            ArtistaId = artistaId
        }).ToList();
        _context.ProductoArtistas.AddRange(artistasNuevos);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin, Empleado")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto is null) return NotFound();

        var artistas = _context.ProductoArtistas
            .Where(pa => pa.ProductoId == id);
        _context.ProductoArtistas.RemoveRange(artistas);

        var inventario = await _context.Inventarios
            .FirstOrDefaultAsync(i => i.ProductoId == id);
        if (inventario is not null)
            _context.Inventarios.Remove(inventario);

        
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
        return NoContent();

    }
}




