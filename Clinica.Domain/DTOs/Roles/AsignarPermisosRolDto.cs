namespace Clinica.Domain.DTOs.Roles;

public class AsignarPermisosRolDto
{
    public Guid RolId { get; set; }
    public List<Guid> PermisosIds { get; set; } = new();
}