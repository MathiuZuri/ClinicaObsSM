using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class AjusteFinancieroRepository : GenericRepository<AjusteFinanciero>, IAjusteFinancieroRepository
{
    public AjusteFinancieroRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AjusteFinanciero>> ObtenerTodosConDetalleAsync()
    {
        return await Context.AjustesFinancieros
            .Include(x => x.Pago)
            .Include(x => x.Paciente)
            .Include(x => x.Atencion)
            .Include(x => x.UsuarioRegistro)
            .OrderByDescending(x => x.FechaRegistro)
            .ToListAsync();
    }

    public async Task<IEnumerable<AjusteFinanciero>> ObtenerPorAtencionAsync(Guid atencionId)
    {
        return await Context.AjustesFinancieros
            .Include(x => x.Pago)
            .Include(x => x.Paciente)
            .Include(x => x.Atencion)
            .Include(x => x.UsuarioRegistro)
            .Where(x => x.AtencionId == atencionId)
            .OrderByDescending(x => x.FechaRegistro)
            .ToListAsync();
    }

    public async Task<IEnumerable<AjusteFinanciero>> ObtenerPorPagoAsync(Guid pagoId)
    {
        return await Context.AjustesFinancieros
            .Include(x => x.Pago)
            .Include(x => x.Paciente)
            .Include(x => x.Atencion)
            .Include(x => x.UsuarioRegistro)
            .Where(x => x.PagoId == pagoId)
            .OrderByDescending(x => x.FechaRegistro)
            .ToListAsync();
    }
}