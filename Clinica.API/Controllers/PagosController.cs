using Clinica.API.Services;
using Clinica.Domain.DTOs.Pagos;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PagosController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public PagosController(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        return Ok(await _pagoService.ObtenerPorPacienteAsync(pacienteId));
    }

    [HttpGet("cita/{citaId:guid}")]
    public async Task<IActionResult> GetByCita(Guid citaId)
    {
        return Ok(await _pagoService.ObtenerPorCitaAsync(citaId));
    }

    [HttpGet("atencion/{atencionId:guid}")]
    public async Task<IActionResult> GetByAtencion(Guid atencionId)
    {
        return Ok(await _pagoService.ObtenerPorAtencionAsync(atencionId));
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPagoDto dto)
    {
        try
        {
            var id = await _pagoService.RegistrarAsync(dto);
            return Ok(new { mensaje = "Pago registrado correctamente.", id });
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