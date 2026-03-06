//dependencias
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs;
using MiPrimeraAPI.DTOs.Categoria;
using MiPrimeraAPI.Models;

namespace MiPrimeraAPI.Controllers;

//Atributos del controlador
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    //Inyección de dependencias (no se puede reasignar el "_context")
    private readonly MediaStoreContext _context;

    public CategoriasController(MediaStoreContext context)
    {
        _context = context;
    }

    // GET api/categorias
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaResponseDto>>> GetAll()
    {
        //Obtiene las categorías de la base de datos
        var categorias = await _context.Categorias.ToListAsync();

        // Mapea las categorías a DTOs de respuesta
        var response = categorias.Select(c => new CategoriaResponseDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Icono = c.Icono,
            PermitePrestamo = c.PermitePrestamo
        });

        return Ok(response);
    }

    // GET api/categorias/1
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaResponseDto>> GetById(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();

        return Ok(new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Icono = categoria.Icono,
            PermitePrestamo = categoria.PermitePrestamo
        });
    }

    // POST api/categorias
    [Authorize(Roles = "Admin,Empleado")]
    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> Create(CategoriaCreateDto dto)
    {
        var categoria = new Categoria
        {
            Nombre = dto.Nombre,
            Icono = dto.Icono,
            PermitePrestamo = dto.PermitePrestamo
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        var response = new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Icono = categoria.Icono,
            PermitePrestamo = categoria.PermitePrestamo
        };

        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, response);
    }

    [Authorize(Roles = "Admin,Empleado")]
    // PUT api/categorias/1
    [HttpPut("{id}")]
    // Se pide el id en la ruta y el DTO en el cuerpo de la solicitud
    public async Task<IActionResult> Update(int id, CategoriaCreateDto dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();

        categoria.Nombre = dto.Nombre;
        categoria.Icono = dto.Icono;
        categoria.PermitePrestamo = dto.PermitePrestamo;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Admin,Empleado")]
    // DELETE api/categorias/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}