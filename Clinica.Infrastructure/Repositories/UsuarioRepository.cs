using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return await Context.Usuarios
            .Include(x => x.UsuarioRoles)
            .ThenInclude(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo == correo);
    }

    public async Task<Usuario?> ObtenerPorUserNameAsync(string userName)
    {
        return await Context.Usuarios
            .Include(x => x.UsuarioRoles)
            .ThenInclude(x => x.Rol)
            .FirstOrDefaultAsync(x => x.UserName == userName);
    }
}