using Clinica.Domain.Entities;

namespace Clinica.Domain.Interfaces;

public interface ICitaRepository : IGenericRepository<Cita>
{
    // Métodos específicos que la clínica necesita para operar
    Task<IReadOnlyList<Cita>> ObtenerCitasPorPacienteAsync(Guid pacienteId);
    Task<IReadOnlyList<Cita>> ObtenerCitasDelDiaPorMedicoAsync(Guid personalMedicoId, DateTime fecha);
    Task<bool> ExisteInterferenciaDeHorarioAsync(Guid personalMedicoId, DateTime fechaHora, int duracionMinutos);
}

//(Deberás crear interfaces similares como IUsuarioRepository para buscar usuarios por correo al iniciar sesión)