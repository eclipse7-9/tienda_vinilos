using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Artista;
using MiPrimeraAPI.Models;


namespace MiPrimeraAPI.Controllers;

//Atributos del controlador
[ApiController]
[Route("api/[controller]")]
public class ArtistasController : ControllerBase
{
    //Inyección de dependencias (no se puede reasignar el "_context")
    private readonly MediaStoreContext _context;

    public ArtistasController(MediaStoreContext context)
    {
        _context = context;
    }
    // GET api/artistas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtistaResponseDto>>> GetAll()
    {
        var artistas = await _context.Artistas.ToListAsync();
        var response = artistas.Select(c => new ArtistaResponseDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Biografia = c.Biografia,
            Tipo = c.Tipo
        });
        return Ok(response);
    }

    // GET api/artistas/1
    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistaResponseDto>> GetById(int id)
    {
        var artista = await _context.Artistas.FindAsync(id);
        if (artista is null) return NotFound();

        return Ok(new ArtistaResponseDto
        {
            Id = artista.Id,
            Nombre = artista.Nombre,
            Biografia = artista.Biografia,
            Tipo = artista.Tipo
        });
    }

    // POST api/artistas
    [HttpPost]
    public async Task<ActionResult<ArtistaResponseDto>> Create(ArtistaCreateDto dto)
    {
        var artista = new Artista
        {
            Nombre = dto.Nombre,
            Biografia = dto.Biografia,
            Tipo = dto.Tipo
        };

        _context.Artistas.Add(artista);
        await _context.SaveChangesAsync();

        var response = new ArtistaResponseDto
        {
            Id = artista.Id,
            Nombre = artista.Nombre,
            Biografia = artista.Biografia,
            Tipo = artista.Tipo
        };

        return CreatedAtAction(nameof(GetById), new { id = artista.Id }, response);
    }

    // PUT api/artistas/1
    [HttpPut("{id}")]
    // Se pide el id en la ruta y el DTO en el cuerpo de la solicitud
    public async Task<IActionResult> Update(int id, ArtistaCreateDto dto)
    {
        var artista = await _context.Artistas.FindAsync(id);
        if (artista is null) return NotFound();

        artista.Nombre = dto.Nombre;
        artista.Biografia = dto.Biografia;
        artista.Tipo = dto.Tipo;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/artistas/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var artista = await _context.Artistas.FindAsync(id);
        if (artista is null) return NotFound();

        _context.Artistas.Remove(artista);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}