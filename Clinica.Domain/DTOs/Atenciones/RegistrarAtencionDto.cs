namespace Clinica.Domain.DTOs.Atenciones;

public class RegistrarAtencionDto
{
    public Guid PacienteId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ServicioClinicoId { get; set; }
    public Guid? CitaId { get; set; }
    public Guid HistorialClinicoId { get; set; }

    public string MotivoConsulta { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? DiagnosticoResumen { get; set; }
    public string? Indicaciones { get; set; }
    public string? Tratamiento { get; set; }

    public decimal CostoFinal { get; set; }

    public Guid? UsuarioRegistroId { get; set; }
}