using Clinica.API.Services;
using Clinica.Domain.DTOs.Pacientes;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _pacienteService;

    public PacientesController(IPacienteService pacienteService)
    {
        _pacienteService = pacienteService;
    }

    [Authorize(Policy = PermisosPolicies.PacienteVer)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _pacienteService.ObtenerTodosAsync());
    }

    [Authorize(Policy = PermisosPolicies.PacienteVer)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var paciente = await _pacienteService.ObtenerPorIdAsync(id);
        return paciente == null ? NotFound(new { mensaje = "Paciente no encontrado." }) : Ok(paciente);
    }

    [Authorize(Policy = PermisosPolicies.PacienteVer)]
    [HttpGet("dni/{dni}")]
    public async Task<IActionResult> GetByDni(string dni)
    {
        var paciente = await _pacienteService.ObtenerPorDniAsync(dni);
        return paciente == null ? NotFound(new { mensaje = "Paciente no encontrado." }) : Ok(paciente);
    }

    [Authorize(Policy = PermisosPolicies.PacienteCrear)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearPacienteDto dto)
    {
        try
        {
            var id = await _pacienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { mensaje = "Paciente registrado correctamente.", id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [Authorize(Policy = PermisosPolicies.PacienteEditar)]
    [HttpPut("{id:guid}/contacto")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] ActualizarContactoPacienteDto dto)
    {
        try
        {
            await _pacienteService.ActualizarContactoAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}