namespace Clinica.WASM.Services.Auth;

public class AuthStateService
{
    private readonly TokenStorageService _tokenStorage;

    public AuthStateService(TokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public async Task<bool> EstaAutenticadoAsync()
    {
        var token = await _tokenStorage.ObtenerTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<bool> TienePermisoAsync(string permiso)
    {
        var permisos = await _tokenStorage.ObtenerPermisosAsync();
        return permisos.Contains(permiso);
    }

    public async Task<bool> TieneRolAsync(string rol)
    {
        var roles = await _tokenStorage.ObtenerRolesAsync();
        return roles.Contains(rol);
    }

    public async Task<string> ObtenerCodigoUsuarioAsync()
    {
        return await _tokenStorage.ObtenerCodigoUsuarioAsync() ?? "SIN-CODIGO";
    }

    public async Task<string> ObtenerNombreUsuarioAsync()
    {
        return await _tokenStorage.ObtenerNombreUsuarioAsync() ?? "Usuario";
    }

    public async Task<string> ObtenerCorreoUsuarioAsync()
    {
        return await _tokenStorage.ObtenerCorreoUsuarioAsync() ?? "Sin correo";
    }

    public async Task<string> ObtenerRolPrincipalAsync()
    {
        var roles = await _tokenStorage.ObtenerRolesAsync();
        return roles.FirstOrDefault() ?? "Sin rol";
    }

    public async Task CerrarSesionAsync()
    {
        await _tokenStorage.LimpiarSesionAsync();
    }
}