using Clinica.API.Services;
using Clinica.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pacientes = await _pacienteService.ObtenerTodosAsync();
        return Ok(pacientes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var paciente = await _pacienteService.ObtenerPorIdAsync(id);
        if (paciente == null) return NotFound("Paciente no encontrado.");
        return Ok(paciente);
    }

    [HttpGet("dni/{dni}")]
    public async Task<IActionResult> GetByDni(string dni)
    {
        var paciente = await _pacienteService.ObtenerPorDniAsync(dni);
        if (paciente == null) return NotFound("No se encontró paciente con ese DNI.");
        return Ok(paciente);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearPacienteDto dto)
    {
        try
        {
            var id = await _pacienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { Mensaje = "Paciente registrado con éxito", Id = id });
        }
        catch (InvalidOperationException ex)
        {
            // Atrapa nuestras reglas de negocio (DNI duplicado) y devuelve un Error 400
            return BadRequest(new { Mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}/contacto")]
    public async Task<IActionResult> UpdateContact(Guid id, ActualizarContactoPacienteDto dto)
    {
        try
        {
            await _pacienteService.ActualizarContactoAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Mensaje = ex.Message });
        }
    }
}