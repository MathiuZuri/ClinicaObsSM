namespace Clinica.Domain.Enums;

public enum EstadoCita
{
    Solicitada = 1,  // El paciente la pidió desde la web
    Agendada = 2,    // Recepción le asignó fecha y hora
    Confirmada = 3,  // El paciente confirmó asistencia
    EnEspera = 4,    // El paciente está en la sala de la clínica
    Atendida = 5,    // La obstetra terminó la consulta
    Cancelada = 6,   // Cancelada por el paciente o la clínica
    NoAsistio = 7    // El paciente nunca llegó
}