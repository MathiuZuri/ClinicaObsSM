using Clinica.API.Services;
using Clinica.Domain.DTOs.Doctores;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctoresController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctoresController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _doctorService.ObtenerTodosAsync());
    }

    [HttpGet("activos")]
    public async Task<IActionResult> GetActivos()
    {
        return Ok(await _doctorService.ObtenerActivosAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var doctor = await _doctorService.ObtenerPorIdAsync(id);
        return doctor == null ? NotFound(new { mensaje = "Doctor no encontrado." }) : Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearDoctorDto dto)
    {
        try
        {
            var id = await _doctorService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { mensaje = "Doctor registrado correctamente.", id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EditarDoctorDto dto)
    {
        try
        {
            await _doctorService.ActualizarAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}