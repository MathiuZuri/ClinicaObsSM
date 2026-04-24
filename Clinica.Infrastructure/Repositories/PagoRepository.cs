using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class PagoRepository : GenericRepository<Pago>, IPagoRepository
{
    public PagoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Pago>> ObtenerPorPacienteAsync(Guid pacienteId)
    {
        return await Context.Pagos
            .Include(x => x.Paciente)
            .Include(x => x.ServicioClinico)
            .Include(x => x.Cita)
            .Include(x => x.Atencion)
            .Where(x => x.PacienteId == pacienteId)
            .OrderByDescending(x => x.FechaPago)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pago>> ObtenerPorCitaAsync(Guid citaId)
    {
        return await Context.Pagos
            .Include(x => x.Paciente)
            .Include(x => x.ServicioClinico)
            .Where(x => x.CitaId == citaId)
            .OrderByDescending(x => x.FechaPago)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pago>> ObtenerPorAtencionAsync(Guid atencionId)
    {
        return await Context.Pagos
            .Include(x => x.Paciente)
            .Include(x => x.ServicioClinico)
            .Where(x => x.AtencionId == atencionId)
            .OrderByDescending(x => x.FechaPago)
            .ToListAsync();
    }
}