using Clinica.API.Services;
using Clinica.Domain.DTOs.Horarios;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HorariosController : ControllerBase
{
    private readonly IHorarioDoctorService _horarioService;

    public HorariosController(IHorarioDoctorService horarioService)
    {
        _horarioService = horarioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _horarioService.ObtenerTodosAsync());
    }

    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<IActionResult> GetByDoctor(Guid doctorId)
    {
        return Ok(await _horarioService.ObtenerPorDoctorAsync(doctorId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearHorarioDoctorDto dto)
    {
        try
        {
            var id = await _horarioService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id }, new { mensaje = "Horario registrado correctamente.", id });
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditarHorarioDoctorDto dto)
    {
        try
        {
            await _horarioService.ActualizarAsync(id, dto);
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
}