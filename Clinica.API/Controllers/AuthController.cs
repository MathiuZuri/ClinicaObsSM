using Clinica.API.Services;
using Clinica.Domain.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Clinica.API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Clinica.API.Filters;
using Clinica.Domain.Enums;

namespace Clinica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [Auditoria("Seguridad", "Usuario", TipoAccionAuditoria.Login, NivelAuditoria.Critico)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] IniciarSesionDto dto)
    {
        try
        {
            var respuesta = await _authService.IniciarSesionAsync(dto);
            return Ok(respuesta);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
}