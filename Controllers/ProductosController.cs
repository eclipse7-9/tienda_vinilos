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
    public ProductosController(MediaStoreContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoResponseDto>>> GetAll()
    {
        try {
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Inventario)
                .Include(p => p.ProductoArtistas)
                    .ThenInclude(pa => pa.Artista)
                .ToListAsync();

            var response = productos.Select(p => new ProductoResponseDto
            {
                Id = p.Id,
                Titulo = p.Titulo ?? string.Empty,
                Descripcion = p.Descripcion ?? string.Empty,
                Precio = p.Precio,
                AnioLanzamiento = p.AnioLanzamiento,
                ImagenUrl = p.ImagenUrl ?? string.Empty,
                CategoriaId = p.CategoriaId,
                EstaActivo = p.EstaActivo,
                Categoria = p.Categoria == null ? null : new CategoriaDetalleDto
                {
                    Nombre = p.Categoria.Nombre ?? "Sin Categoría",
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
            }).ToList();

            return Ok(response);
        } catch (Exception ex) {
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoResponseDto>> GetById(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Inventario)
            .Include(p => p.ProductoArtistas)
                .ThenInclude(pa => pa.Artista)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto is null) return NotFound();

        return Ok(new ProductoResponseDto
        {
            Id = producto.Id,
            Titulo = producto.Titulo ?? string.Empty,
            Descripcion = producto.Descripcion ?? string.Empty,
            Precio = producto.Precio,
            AnioLanzamiento = producto.AnioLanzamiento,
            ImagenUrl = producto.ImagenUrl ?? string.Empty,
            CategoriaId = producto.CategoriaId,
            EstaActivo = producto.EstaActivo,
            Categoria = producto.Categoria == null ? null : new CategoriaDetalleDto
            {
                Nombre = producto.Categoria.Nombre ?? "Sin Categoría",
                PermitePrestamo = producto.Categoria.PermitePrestamo
            },
            Inventario = producto.Inventario == null ? null : new InventarioDto
            {
                StockDisponible = producto.Inventario.StockDisponible,
                StockDisponiblePrestamo = producto.Inventario.StockDisponiblePrestamo
            },
            Artistas = producto.ProductoArtistas.Select(pa => new ArtistaDto
            {
                Nombre = pa.Artista?.Nombre ?? "Desconocido"
            }).ToList()
        });
    }

    [Authorize(Roles = "Admin,Empleado")]
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
            EstaActivo = true
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        var inventario = new Inventario
        {
            ProductoId = producto.Id,
            StockTotal = dto.StockInicial,
            StockDisponible = dto.StockInicial,
            StockDisponiblePrestamo = dto.StockDisponiblePrestamo,
            StockMinimo = 5
        };
        _context.Inventarios.Add(inventario);

        if (dto.ArtistaIds != null)
        {
            foreach (var artistaId in dto.ArtistaIds)
            {
                _context.ProductoArtistas.Add(new ProductoArtista
                {
                    ProductoId = producto.Id,
                    ArtistaId = artistaId
                });
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, null);
    }
}
