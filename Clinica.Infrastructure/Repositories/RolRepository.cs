using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class RolRepository : GenericRepository<Rol>, IRolRepository
{
    public RolRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Rol?> ObtenerPorNombreAsync(string nombre)
    {
        return await Context.Roles
            .Include(x => x.RolPermisos)
            .ThenInclude(x => x.Permiso)
            .FirstOrDefaultAsync(x => x.Nombre == nombre);
    }
}