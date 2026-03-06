//dependencias
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Cliente;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;

//Atributos del controlador
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    //Inyección de dependencias (no se puede reasignar el "_context")
    private readonly MediaStoreContext _context;

    public ClientesController(MediaStoreContext context)
    {
        _context = context;
    }

    // GET api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetAll()
    {
        //Obtiene las categorías de la base de datos
        var clientes = await _context.Clientes.ToListAsync();

        // Mapea las categorías a DTOs de respuesta
        var response = clientes.Select(c => new ClienteResponseDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Email = c.Email,
            Telefono = c.Telefono,
            MaxPrestamosSiultaneos = c.MaxPrestamosSiultaneos,
            FechaRegistro = c.FechaRegistro
        });

        return Ok(response);
    }

    // GET api/clientes/1
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteResponseDto>> GetById(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        return Ok(new ClienteResponseDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            MaxPrestamosSiultaneos = cliente.MaxPrestamosSiultaneos,
            FechaRegistro = cliente.FechaRegistro
        });
    }

    // POST api/clientes
    [HttpPost]
    public async Task<ActionResult<ClienteResponseDto>> Create(ClienteCreateDto dto)
    {
        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            FechaRegistro = DateTime.UtcNow
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        var response = new ClienteResponseDto
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            Telefono = cliente.Telefono,
            MaxPrestamosSiultaneos = cliente.MaxPrestamosSiultaneos,
            FechaRegistro = cliente.FechaRegistro
        };

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, response);
    }

    // PUT api/clientes/1
    [HttpPut("{id}")]
    // Se pide el id en la ruta y el DTO en el cuerpo de la solicitud
    public async Task<IActionResult> Update(int id, ClienteCreateDto dto)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        cliente.Nombre = dto.Nombre;
        cliente.Apellido = dto.Apellido;
        cliente.Email = dto.Email;
        cliente.Telefono = dto.Telefono;




        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/clientes/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}