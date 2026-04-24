using Clinica.Domain.Entities;

namespace Clinica.Domain.Interfaces;

public interface ICitaRepository : IGenericRepository<Cita>
{
    Task<bool> ExisteInterferenciaHorarioAsync(
        Guid doctorId,
        DateOnly fecha,
        TimeOnly horaInicio,
        TimeOnly horaFin,
        Guid? citaIdExcluir = null);

    Task<IEnumerable<Cita>> ObtenerPorPacienteAsync(Guid pacienteId);
    Task<IEnumerable<Cita>> ObtenerPorDoctorAsync(Guid doctorId);
}