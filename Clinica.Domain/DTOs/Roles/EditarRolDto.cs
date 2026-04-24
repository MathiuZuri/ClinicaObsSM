namespace Clinica.Domain.DTOs.Roles;

public class EditarRolDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}