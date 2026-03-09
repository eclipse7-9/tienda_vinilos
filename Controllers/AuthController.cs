using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraAPI.Data;
using MiPrimeraAPI.DTOs.Auth;
using MiPrimeraAPI.Models;
using MiPrimeraAPI.Services;


namespace MiPrimeraAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly MediaStoreContext _context;
    private readonly IConfiguration _config;
    private readonly TokenService _tokenService;

    public AuthController(MediaStoreContext context, IConfiguration config, TokenService tokenService)
    {
        _context = context;
        _config = config;
        _tokenService = tokenService;
    }


    [HttpPost("register")]

    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)

    {
        var emailRep = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (emailRep != null)
            {
            return BadRequest("El email ya está registrado");
        }
        
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var nuevoUsuario = new Usuario
        {
            Email = dto.Email,
            PasswordHash = hash,
            Rol = RolUsuario.Cliente,
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        var Cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            FechaRegistro = DateTime.UtcNow,
            UsuarioId = nuevoUsuario.Id
        };

        _context.Clientes.Add(Cliente);
        await _context.SaveChangesAsync();

        var response = new AuthResponseDto
        {
            Email = nuevoUsuario.Email,
            Rol = RolUsuario.Cliente.ToString(),
            Token = _tokenService.GenerarToken(nuevoUsuario)
        };

        return Ok(response);
    }

    [HttpPost("login")]

    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var existe = await _context.Usuarios
            .FirstOrDefaultAsync(e => e.Email == dto.Email);
        if (existe == null) return BadRequest("El email no está registrado en la página");

        var hash = BCrypt.Net.BCrypt.Verify(dto.Password, existe.PasswordHash);

        if (!hash) return BadRequest("contraseña incorrecta");

        var response = new AuthResponseDto
        {
            Email = dto.Email,
            Rol = RolUsuario.Cliente.ToString(),
            Token = _tokenService.GenerarToken(existe)
        }; return Ok(response);
            
    }






}
