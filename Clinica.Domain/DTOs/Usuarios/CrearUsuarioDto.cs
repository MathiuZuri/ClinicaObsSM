namespace Clinica.Domain.DTOs.Usuarios;

public class CrearUsuarioDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}