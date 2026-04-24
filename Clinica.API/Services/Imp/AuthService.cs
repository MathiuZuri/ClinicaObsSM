using Clinica.Domain.DTOs.Auth;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<RespuestaInicioSesionDto> IniciarSesionAsync(IniciarSesionDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(dto.UsuarioOCorreo)
                      ?? await _usuarioRepository.ObtenerPorUserNameAsync(dto.UsuarioOCorreo);

        if (usuario == null)
            throw new InvalidOperationException("Usuario o contraseña incorrectos.");

        // Temporal: luego reemplazamos por BCrypt/JWT real.
        if (usuario.PasswordHash != dto.Password)
            throw new InvalidOperationException("Usuario o contraseña incorrectos.");

        var roles = usuario.UsuarioRoles
            .Where(x => x.Activo)
            .Select(x => x.Rol.Nombre)
            .ToList();

        return new RespuestaInicioSesionDto
        {
            UsuarioId = usuario.Id,
            CodigoUsuario = usuario.CodigoUsuario,
            NombreCompleto = $"{usuario.Nombres} {usuario.Apellidos}",
            Correo = usuario.Correo,
            Token = "TOKEN_TEMPORAL",
            Roles = roles,
            Permisos = new List<string>()
        };
    }
}