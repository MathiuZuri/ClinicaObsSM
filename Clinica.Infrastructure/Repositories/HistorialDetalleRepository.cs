using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;

namespace Clinica.Infrastructure.Repositories;

public class HistorialDetalleRepository 
    : GenericRepository<HistorialDetalle>, IHistorialDetalleRepository
{
    public HistorialDetalleRepository(ApplicationDbContext context) : base(context)
    {
    }
}