namespace Clinica.API.Services;

public interface IUsuarioActualService
{
    Guid ObtenerUsuarioId();
    Guid? ObtenerUsuarioIdOpcional();
}