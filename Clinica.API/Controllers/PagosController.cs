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

    [Authorize(Policy = PermisosPolicies.PagoVer)]
    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        return Ok(await _pagoService.ObtenerPorPacienteAsync(pacienteId));
    }

    [Authorize(Policy = PermisosPolicies.PagoVer)]
    [HttpGet("cita/{citaId:guid}")]
    public async Task<IActionResult> GetByCita(Guid citaId)
    {
        return Ok(await _pagoService.ObtenerPorCitaAsync(citaId));
    }

    [Authorize(Policy = PermisosPolicies.PagoVer)]
    [HttpGet("atencion/{atencionId:guid}")]
    public async Task<IActionResult> GetByAtencion(Guid atencionId)
    {
        return Ok(await _pagoService.ObtenerPorAtencionAsync(atencionId));
    }

    [Authorize(Policy = PermisosPolicies.PagoRegistrar)]
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