using Clinica.API.Services;
using Clinica.Domain.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;


namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [Authorize(Policy = PermisosPolicies.UsuarioVer)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _usuarioService.ObtenerTodosAsync());
    }

    [Authorize(Policy = PermisosPolicies.UsuarioVer)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var usuario = await _usuarioService.ObtenerPorIdAsync(id);
        return usuario == null ? NotFound(new { mensaje = "Usuario no encontrado." }) : Ok(usuario);
    }

    [Authorize(Policy = PermisosPolicies.UsuarioCrear)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearUsuarioDto dto)
    {
        try
        {
            var id = await _usuarioService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { mensaje = "Usuario creado correctamente.", id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [Authorize(Policy = PermisosPolicies.UsuarioEditar)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditarUsuarioDto dto)
    {
        try
        {
            await _usuarioService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [Authorize(Policy = PermisosPolicies.UsuarioAsignarRol)]
    [HttpPost("asignar-rol")]
    public async Task<IActionResult> AssignRole([FromBody] AsignarRolUsuarioDto dto)
    {
        try
        {
            await _usuarioService.AsignarRolAsync(dto);
            return Ok(new { mensaje = "Rol asignado correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}