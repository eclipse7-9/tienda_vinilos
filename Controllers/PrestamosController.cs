using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Prestamo;
using MiPrimeraAPI.DTOs.Venta;
using MiPrimeraAPI.Models;
using System.Xml;

namespace MiPrimeraAPI.Controllers;

[Authorize]

[ApiController]
[Route("api/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly MediaStoreContext _context;
    public PrestamosController(MediaStoreContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Empleado")]
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

    [Authorize(Roles = "Admin,Empleado")]
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PrestamoResponseDto>> Create(PrestamoCreateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente is null) return NotFound("Cliente no encontrado");

        var limite = await _context.Prestamos.Where(p => p.ClienteId == dto.ClienteId && p.Estado == EstadoPrestamo.Activo).CountAsync();
        if (limite >= 3) return BadRequest("El cliente tiene el limite de prestamos activos alcanzado");

        var prestamo = new Prestamo
        {
            ClienteId = dto.ClienteId,
            Estado = EstadoPrestamo.Activo,
            FechaPrestamo = DateTime.Now,
            FechaDevolucionEsperada = DateTime.Now.AddDays(7),
            Multa = null,
        };
        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();

        foreach (var detalleDto in dto.Detalles)
        {
            var producto = await _context.Productos.
                Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == detalleDto.ProductoId);
            if (producto is null) return NotFound($"Producto no encontrado para el prestamo");

            var cantidad = await _context.Inventarios
                .FirstOrDefaultAsync(i => i.ProductoId == detalleDto.ProductoId);

            if (cantidad is null || cantidad.StockDisponiblePrestamo <= 0)
                return BadRequest($"No hay stock disponible para préstamo del producto: {producto.Titulo}");

            if (!producto.Categoria.PermitePrestamo)
                return BadRequest($"El producto {producto.Titulo} no se puede prestar porque su categoría no lo permite");


            cantidad.StockDisponiblePrestamo -= 1;
            cantidad.StockEnPrestamo += 1;

            var detalle = new PrestamoDetalle
            {
                PrestamoId = prestamo.Id,
                ProductoId = detalleDto.ProductoId,
                Observaciones = null
            };
            _context.PrestamoDetalles.Add(detalle);


        }
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = prestamo.Id }, null);
    }

    [Authorize(Roles = "Admin,Empleado")]
    [HttpPut("{id}/devolver")]
    public async Task<IActionResult> Devolver(int id)
    {
        var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo is null) return BadRequest();

        prestamo.FechaDevolucionReal = DateTime.Now;
        if (prestamo.FechaDevolucionReal > prestamo.FechaDevolucionEsperada)
        {
            var atraso = (prestamo.FechaDevolucionReal.Value - prestamo.FechaDevolucionEsperada).Days;

            prestamo.Multa = atraso * 10000;
        }
        else
        {
            prestamo.Multa = 0;
        }
            
        prestamo.Estado = EstadoPrestamo.Devuelto;
        var detalles = await _context.PrestamoDetalles.Where(d => d.PrestamoId == id).ToListAsync();

        foreach (var detalle in detalles)
        {
            var cantidad = await _context.Inventarios
                  .FirstOrDefaultAsync(i => i.ProductoId == detalle.ProductoId);

            if (cantidad is not null)
                {
                cantidad.StockDisponiblePrestamo += 1;
                cantidad.StockEnPrestamo -= 1;
            }
        }
       
        await _context.SaveChangesAsync();
        return NoContent();
    }



}


