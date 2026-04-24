using Clinica.API.Services;
using Clinica.Domain.DTOs.Atenciones;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtencionesController : ControllerBase
{
    private readonly IAtencionService _atencionService;

    public AtencionesController(IAtencionService atencionService)
    {
        _atencionService = atencionService;
    }

    [Authorize(Policy = PermisosPolicies.AtencionVer)]
    [HttpGet("paciente/{pacienteId:guid}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        return Ok(await _atencionService.ObtenerPorPacienteAsync(pacienteId));
    }

    [Authorize(Policy = PermisosPolicies.AtencionVer)]
    [HttpGet("cita/{citaId:guid}")]
    public async Task<IActionResult> GetByCita(Guid citaId)
    {
        var atencion = await _atencionService.ObtenerPorCitaAsync(citaId);
        return atencion == null ? NotFound(new { mensaje = "Atención no encontrada." }) : Ok(atencion);
    }

    [Authorize(Policy = PermisosPolicies.AtencionRegistrar)]
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarAtencionDto dto)
    {
        try
        {
            var id = await _atencionService.RegistrarAsync(dto);
            return Ok(new { mensaje = "Atención registrada correctamente.", id });
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

    [Authorize(Policy = PermisosPolicies.AtencionCerrar)]
    [HttpPut("{id:guid}/cerrar")]
    public async Task<IActionResult> Cerrar(Guid id, [FromBody] CerrarAtencionDto dto)
    {
        try
        {
            await _atencionService.CerrarAsync(id, dto);
            return Ok(new { mensaje = "Atención cerrada correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }
}