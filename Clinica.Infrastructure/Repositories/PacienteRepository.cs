using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class PacienteRepository : GenericRepository<Paciente>, IPacienteRepository
{
    public PacienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Paciente?> ObtenerPorDniAsync(string dni)
    {
        return await _context.Set<Paciente>()
            .FirstOrDefaultAsync(p => p.DNI == dni);
    }

    public async Task<Paciente?> ObtenerPorNumeroHcAsync(string numeroHc)
    {
        return await _context.Set<Paciente>()
            .FirstOrDefaultAsync(p => p.NumeroHC == numeroHc);
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        return await _context.Set<Paciente>()
            .AnyAsync(p => p.DNI == dni);
    }

    public async Task<bool> ExisteNumeroHcAsync(string numeroHc)
    {
        return await _context.Set<Paciente>()
            .AnyAsync(p => p.NumeroHC == numeroHc);
    }
    
    public async Task<int> ContarPacientesPorAnioAsync(int anio)
    {
        return await _context.Set<Paciente>()
            .CountAsync(p => p.FechaCreacion.Year == anio);
    }
}