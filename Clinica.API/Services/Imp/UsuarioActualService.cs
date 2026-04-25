using System.Security.Claims;

namespace Clinica.API.Services.Imp;

public class UsuarioActualService : IUsuarioActualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid ObtenerUsuarioId()
    {
        var usuarioId = ObtenerUsuarioIdOpcional();

        if (usuarioId == null)
            throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");

        return usuarioId.Value;
    }

    public Guid? ObtenerUsuarioIdOpcional()
    {
        var claim = _httpContextAccessor.HttpContext?
            .User
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        return Guid.TryParse(claim, out var usuarioId)
            ? usuarioId
            : null;
    }
}