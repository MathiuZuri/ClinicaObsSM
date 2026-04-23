namespace Clinica.Domain.DTOs;

public class CrearCitaDto
{
    public Guid PacienteId { get; set; }
    public Guid? PersonalMedicoId { get; set; }
    public DateTime FechaHoraProgramada { get; set; } = DateTime.Now.AddDays(1);
    public string Servicio { get; set; } = string.Empty;
    public string? MotivoConsulta { get; set; }
    public string? Notas { get; set; }
}