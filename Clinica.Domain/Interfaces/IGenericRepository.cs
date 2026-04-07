using Clinica.Domain.DTOs;

namespace Clinica.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> ObtenerPorIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListarTodosAsync();
    Task AgregarAsync(T entity);
    Task ActualizarAsync(T entity); 
    Task EliminarAsync(T entity);
    Task SaveChangesAsync();
}