using Clinica.Domain.Enums;

namespace Clinica.Domain.Entities;

public class Cita
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid? PersonalMedicoId { get; set; }
    
    public DateTime? FechaHoraProgramada { get; set; }
    public int DuracionMinutos { get; set; }
    
    public string Servicio { get; set; } = string.Empty;
    public string? MotivoConsulta { get; set; }
    public string? Notas { get; set; }
    
    public EstadoCita Estado { get; set; }
    public DateTime FechaRegistro { get; set; }
    
    protected Cita() { }

    // Constructor para cuando un paciente SOLICITA una cita desde la web
    public Cita(Guid pacienteId, string servicio, string? motivoConsulta)
    {
        Id = Guid.NewGuid();
        PacienteId = pacienteId;
        Servicio = servicio;
        MotivoConsulta = motivoConsulta;
        Estado = EstadoCita.Solicitada;
        FechaRegistro = DateTime.UtcNow;
        DuracionMinutos = 30; // Duración por defecto
    }

    // Recepción AGENDA la cita
    public void Agendar(Guid personalMedicoId, DateTime fechaHora, int duracionMinutos = 30)
    {
        // No se puede agendar en el pasado
        if (fechaHora < DateTime.UtcNow)
            throw new ArgumentException("La fecha programada no puede estar en el pasado.");

        PersonalMedicoId = personalMedicoId;
        FechaHoraProgramada = fechaHora;
        DuracionMinutos = duracionMinutos;
        Estado = EstadoCita.Agendada;
    }

    // Métodos para avanzar el flujo
    public void MarcarComoEnEspera() => Estado = EstadoCita.EnEspera;
    public void MarcarComoAtendida() => Estado = EstadoCita.Atendida;
    public void Cancelar() => Estado = EstadoCita.Cancelada;
}