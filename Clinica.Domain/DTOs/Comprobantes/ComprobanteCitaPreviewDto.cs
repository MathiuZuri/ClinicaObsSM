namespace Clinica.Domain.DTOs.Comprobantes;

public class ComprobanteCitaPreviewDto
{
    public Guid CitaId { get; set; }
    public string CodigoCita { get; set; } = string.Empty;

    public Guid PacienteId { get; set; }
    public string Paciente { get; set; } = string.Empty;
    public string DniPaciente { get; set; } = string.Empty;

    public string Doctor { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;

    public DateOnly Fecha { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }

    public string EstadoCita { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}