using Clinica.Domain.Entities;

namespace Clinica.Domain.Interfaces;

public interface IRolRepository : IGenericRepository<Rol>
{
    Task<Rol?> ObtenerPorNombreAsync(string nombre);
}