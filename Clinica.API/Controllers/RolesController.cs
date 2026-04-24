using Clinica.API.Services;
using Clinica.Domain.DTOs.Roles;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        _rolService = rolService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _rolService.ObtenerTodosAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rol = await _rolService.ObtenerPorIdAsync(id);
        return rol == null ? NotFound(new { mensaje = "Rol no encontrado." }) : Ok(rol);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearRolDto dto)
    {
        try
        {
            var id = await _rolService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { mensaje = "Rol creado correctamente.", id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditarRolDto dto)
    {
        try
        {
            await _rolService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("asignar-permisos")]
    public async Task<IActionResult> AssignPermissions([FromBody] AsignarPermisosRolDto dto)
    {
        try
        {
            await _rolService.AsignarPermisosAsync(dto);
            return Ok(new { mensaje = "Permisos asignados correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}