namespace Clinica.Domain.Entities;

public class PersonalMedico
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Especialidad { get;  set; } = string.Empty;
    public string CMP { get; set; } = string.Empty;

    protected PersonalMedico() { }

    public PersonalMedico(Guid usuarioId, string nombres, string apellidos, string especialidad, string cmp)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Nombres = nombres.ToUpper().Trim();
        Apellidos = apellidos.ToUpper().Trim();
        Especialidad = especialidad;
        CMP = cmp;
    }
}