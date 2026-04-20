using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Categoria;
using MiPrimeraAPI.DTOs.Venta;
using MiPrimeraAPI.Models;
using System.Runtime.InteropServices;

namespace MiPrimeraAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly MediaStoreContext _context;

    public VentasController(MediaStoreContext context)
    {
        _context = context;
    }

    // GET
    [Authorize(Roles = "Admin,Empleado")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetAll()
    {
        var ventas = await _context.Ventas.ToListAsync();

        var response = ventas.Select(c => new VentaResponseDto
        {
            Id = c.Id,
            Fecha = c.Fecha,
            Total = c.Total,
            MetodoPago = c.MetodoPago,
            Estado = c.Estado,

            ClienteId = c.ClienteId
        });
        return Ok(response);
    }

    [Authorize(Roles = "Admin,Empleado")]
    [HttpGet("{id}")]
    public async Task<ActionResult<VentaResponseDto>> GetById(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venta == null) return NotFound();

        return Ok(new VentaResponseDto
        {
            Id = venta.Id,
            Fecha = venta.Fecha,
            Total = venta.Total,
            MetodoPago = venta.MetodoPago,
            Estado = venta.Estado,
            ClienteId = venta.ClienteId,

            Detalles = venta.Detalles.Select(d => new VentaDetalleResponseDto
            {
                ProductoId = d.ProductoId,
                TituloProducto = d.Producto.Titulo,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList()
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<VentaResponseDto>> Create(VentaCreateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente is null) return NotFound("Cliente no encontrado");

        string metodoPagoNombre = dto.MetodoPago ?? "No especificado";
        if (dto.MetodoPagoId.HasValue)
        {
            var mp = await _context.MetodoPago.FindAsync(dto.MetodoPagoId.Value);
            if (mp != null) metodoPagoNombre = mp.Tipo;
        }

        var venta = new Venta
        {
            ClienteId = dto.ClienteId,
            MetodoPago = metodoPagoNombre,
            MetodoPagoId = dto.MetodoPagoId,
            DireccionId = dto.DireccionId,
            Estado = "Pendiente",
            Fecha = DateTime.UtcNow,
            Total = 0
        };
        _context.Ventas.Add(venta);

        await _context.SaveChangesAsync();

        decimal total = 0;

        foreach (var detalleDto in dto.Detalles)
        {
            var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
            if (producto is null) return NotFound($"Producto no encontrado/producto agotado");

            var inventario = await _context.Inventarios.
                FirstOrDefaultAsync(i => i.ProductoId == detalleDto.ProductoId);
            if (inventario is null || inventario.StockDisponible < detalleDto.Cantidad)
                return BadRequest("Stock insuficiente");

            //subtotal

            var subtotal = detalleDto.Cantidad * producto.Precio;

            var detalle = new VentaDetalle
            {
                VentaId = venta.Id,
                ProductoId = detalleDto.ProductoId,
                Cantidad = detalleDto.Cantidad,
                PrecioUnitario = producto.Precio,
                Subtotal = subtotal
            };
            _context.VentaDetalles.Add(detalle);

            inventario.StockDisponible -= detalleDto.Cantidad;
            inventario.StockTotal -= detalleDto.Cantidad;
            total += subtotal;  
        }
        venta.Total = total;

        // Crear notificación
        var notif = new Notificacion
        {
            ClienteId = venta.ClienteId,
            Titulo = "🛍️ Compra exitosa",
            Mensaje = $"Tu orden #{venta.Id} por ${total:N0} ha sido procesada.",
            Fecha = DateTime.UtcNow,
            Leida = false
        };
        _context.Notificaciones.Add(notif);

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, null);
    }

    [Authorize]
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<VentaResponseDto>>> GetByCliente(int clienteId)
    {
        var ventas = await _context.Ventas
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        var response = ventas.Select(v => new VentaResponseDto
        {
            Id = v.Id,
            Fecha = v.Fecha,
            Total = v.Total,
            MetodoPago = v.MetodoPago,
            Estado = v.Estado,
            ClienteId = v.ClienteId,
            Detalles = v.Detalles.Select(d => new VentaDetalleResponseDto
            {
                ProductoId = d.ProductoId,
                TituloProducto = d.Producto.Titulo,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList()
        }).ToList();

        return Ok(response);
    }


}