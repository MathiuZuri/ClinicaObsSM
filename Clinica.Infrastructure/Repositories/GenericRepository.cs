using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;


public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T?> ObtenerPorIdAsync(Guid id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListarTodosAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task AgregarAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public Task ActualizarAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task EliminarAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}