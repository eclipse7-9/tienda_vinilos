using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Prestamo;
using MiPrimeraAPI.DTOs.Venta;
using MiPrimeraAPI.Models;
using System.Xml;

namespace MiPrimeraAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public PrestamosController(MediaStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrestamoResponseDto>>> GetAll()
    {
        var prestamos = await _context.Prestamos.ToListAsync();

        var response = prestamos.Select(c => new PrestamoResponseDto
        {
            Id = c.Id,
            FechaPrestamo = c.FechaPrestamo,
            FechaDevolucionEsperada = c.FechaDevolucionEsperada,
            FechaDevolucionReal = c.FechaDevolucionReal,
            Estado = c.Estado.ToString(),
            Multa = c.Multa,

            ClienteId = c.ClienteId
        });
        return Ok(response);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PrestamoResponseDto>> GetById(int id)

    {
        var prestamo = await _context.Prestamos
            .Include(p => p.Detalles)
            .ThenInclude(p => p.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (prestamo == null) return NotFound();

        return Ok(new PrestamoResponseDto
        {
            Id = prestamo.Id,
            FechaPrestamo = prestamo.FechaPrestamo,
            FechaDevolucionEsperada = prestamo.FechaDevolucionEsperada,
            FechaDevolucionReal = prestamo.FechaDevolucionReal,
            Estado = prestamo.Estado.ToString(),
            Multa = prestamo.Multa,
            ClienteId = prestamo.ClienteId,

            Detalles = prestamo.Detalles.Select(d => new PrestamoDetalleResponseDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                TituloProducto = d.Producto.Titulo,
                Observaciones = d.Observaciones
            }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<PrestamoResponseDto>> Create(PrestamoCreateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente is null) return NotFound("Cliente no encontrado");

        var prestamo = new Prestamo
        {
            ClienteId = dto.ClienteId,
            Estado = dto.Estado,
            FechaPrestamo = DateTime.Now,
            Multa = null,
        };
        _context.Ventas.Add(prestamo);
        await _context.SaveChangesAsync();
        var fechaDevolucionEsperada = DateTime.Now.AddDays(7);

        foreach (var detalleDto in dto.Detalles)
        {
            prestamo.FechaDevolucionEsperada = fechaDevolucionEsperada;
            if (prestamo.FechaDevolucionReal > fechaDevolucionEsperada) return BadRequest("La fecha de devolución real no puede ser antes de la fecha de préstamo");

            var cantidad = await _context.PrestamoDetalles.Where(i => i.ProductoId == detalleDto.ProductoId).Select(i => i.StockDisponible).FirstOrDefaultAsync();

            var  producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
            if (producto is null) return NotFound($"Producto no encontrado para el prestamo");

            var inventario = await _context.Inventarios.
                FirstOrDefaultAsync(i => i.ProductoId == detalleDto.ProductoId);
            if (inventario is null) return BadRequest($"No hay stock disponible para el producto {producto.Titulo}");

            

            var detalle = new PrestamoDetalle
            {
                PrestamoId = prestamo.Id,
                ProductoId = detalleDto.ProductoId,
                Observaciones = detalleDto.Observaciones
            };
            _context.PrestamoDetalles.Add(detalle);

            
        }
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = prestamo.Id }, null);

    }

}
