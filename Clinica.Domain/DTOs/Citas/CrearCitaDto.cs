namespace Clinica.Domain.DTOs.Citas;

public class CrearCitaDto
{
    public Guid PacienteId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServicioClinicoId { get; set; }
    public Guid? HorarioDoctorId { get; set; }

    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }

    public string Motivo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }

    public Guid? UsuarioRegistroId { get; set; }
}