using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class CitaRepository : GenericRepository<Cita>, ICitaRepository
{
    public CitaRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Cita>> ObtenerCitasPorPacienteAsync(Guid pacienteId)
    {
        return await _context.Citas
            .Where(c => c.PacienteId == pacienteId)
            .OrderByDescending(c => c.FechaHoraProgramada)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Cita>> ObtenerCitasDelDiaPorMedicoAsync(Guid personalMedicoId, DateTime fecha)
    {
        var inicioDia = fecha.Date;
        var finDia = inicioDia.AddDays(1).AddTicks(-1);

        return await _context.Citas
            .Where(c => c.PersonalMedicoId == personalMedicoId 
                        && c.FechaHoraProgramada >= inicioDia 
                        && c.FechaHoraProgramada <= finDia)
            .OrderBy(c => c.FechaHoraProgramada)
            .ToListAsync();
    }

    public async Task<bool> ExisteInterferenciaDeHorarioAsync(Guid personalMedicoId, DateTime fechaHora, int duracionMinutos)
    {
        var fechaFinNuevaCita = fechaHora.AddMinutes(duracionMinutos);

        // Verifica si ya existe una cita agendada o en espera que se cruce con el nuevo horario propuesto
        return await _context.Citas
            .AnyAsync(c => c.PersonalMedicoId == personalMedicoId
                           && c.FechaHoraProgramada != null
                           && c.Estado != Clinica.Domain.Enums.EstadoCita.Cancelada
                           && c.Estado != Clinica.Domain.Enums.EstadoCita.NoAsistio
                           && c.FechaHoraProgramada < fechaFinNuevaCita 
                           && c.FechaHoraProgramada.Value.AddMinutes(c.DuracionMinutos) > fechaHora);
    }
}