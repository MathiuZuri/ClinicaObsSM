using Clinica.API.Services;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;


namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiciosClinicosController : ControllerBase
{
    private readonly IServicioClinicoService _servicioService;

    public ServiciosClinicosController(IServicioClinicoService servicioService)
    {
        _servicioService = servicioService;
    }

    [Authorize(Policy = PermisosPolicies.ServicioVer)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _servicioService.ObtenerTodosAsync());
    }

    [Authorize(Policy = PermisosPolicies.ServicioVer)]
    [HttpGet("activos")]
    public async Task<IActionResult> GetActivos()
    {
        return Ok(await _servicioService.ObtenerActivosAsync());
    }

    [Authorize(Policy = PermisosPolicies.ServicioVer)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var servicio = await _servicioService.ObtenerPorIdAsync(id);
        return servicio == null ? NotFound(new { mensaje = "Servicio clínico no encontrado." }) : Ok(servicio);
    }
}