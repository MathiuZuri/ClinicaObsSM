namespace Clinica.Domain.DTOs.Auth;

public class IniciarSesionDto
{
    public string UsuarioOCorreo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}