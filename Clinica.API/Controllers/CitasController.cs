using Clinica.Domain.DTOs;
using Clinica.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")] // Esto hace que la ruta base sea: dominio.com/api/citas
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;
    public CitasController(ICitaService citaService) => _citaService = citaService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CitaResponseDto>>> GetAll() 
        => Ok(await _citaService.ObtenerTodasAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<CitaResponseDto>> GetById(Guid id)
    {
        var cita = await _citaService.ObtenerPorIdAsync(id);
        return cita == null ? NotFound() : Ok(cita);
    }

    [HttpPost]
    public async Task<ActionResult<CitaResponseDto>> Create([FromBody] CrearCitaDto dto)
    {
        try {
            var res = await _citaService.AgendarNuevaCitaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
        } catch (InvalidOperationException ex) {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CrearCitaDto dto)
    {
        await _citaService.ActualizarAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _citaService.EliminarAsync(id);
        return NoContent();
    }
}