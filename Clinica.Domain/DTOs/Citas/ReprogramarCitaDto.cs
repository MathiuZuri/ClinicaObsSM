namespace Clinica.Domain.DTOs.Citas;

public class ReprogramarCitaDto
{
    public Guid DoctorId { get; set; }
    public Guid? HorarioDoctorId { get; set; }

    public DateOnly NuevaFecha { get; set; }
    public TimeOnly NuevaHoraInicio { get; set; }
    public TimeOnly NuevaHoraFin { get; set; }

    public string? MotivoReprogramacion { get; set; }
    public Guid? UsuarioRegistroId { get; set; }
}