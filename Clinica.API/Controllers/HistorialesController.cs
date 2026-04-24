using Clinica.API.Services;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HistorialesController : ControllerBase
{
    private readonly IHistorialClinicoService _historialService;

    public HistorialesController(IHistorialClinicoService historialService)
    {
        _historialService = historialService;
    }

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        var historial = await _historialService.ObtenerPorPacienteAsync(pacienteId);
        return historial == null ? NotFound(new { mensaje = "Historial clínico no encontrado." }) : Ok(historial);
    }

    [HttpGet("{historialId:guid}/detalles")]
    public async Task<IActionResult> GetConDetalles(Guid historialId)
    {
        var historial = await _historialService.ObtenerConDetallesAsync(historialId);
        return historial == null ? NotFound(new { mensaje = "Historial clínico no encontrado." }) : Ok(historial);
    }
}