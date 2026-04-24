namespace Clinica.Domain.DTOs.Usuarios;

public class EditarUsuarioDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
}