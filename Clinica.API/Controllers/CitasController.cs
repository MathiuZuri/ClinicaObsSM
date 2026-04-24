using Clinica.API.Services;
using Clinica.Domain.DTOs.Citas;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;

    public CitasController(ICitaService citaService)
    {
        _citaService = citaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _citaService.ObtenerTodasAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cita = await _citaService.ObtenerPorIdAsync(id);
        return cita == null ? NotFound(new { mensaje = "Cita no encontrada." }) : Ok(cita);
    }

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        return Ok(await _citaService.ObtenerPorPacienteAsync(pacienteId));
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId)
    {
        return Ok(await _citaService.ObtenerPorDoctorAsync(doctorId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCitaDto dto)
    {
        try
        {
            var id = await _citaService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { mensaje = "Cita programada correctamente.", id });
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

    [HttpPut("{id:guid}/reprogramar")]
    public async Task<IActionResult> Reprogramar(Guid id, [FromBody] ReprogramarCitaDto dto)
    {
        try
        {
            await _citaService.ReprogramarAsync(id, dto);
            return Ok(new { mensaje = "Cita reprogramada correctamente." });
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

    [HttpPut("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarCitaDto dto)
    {
        try
        {
            await _citaService.CancelarAsync(id, dto);
            return Ok(new { mensaje = "Cita cancelada correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}