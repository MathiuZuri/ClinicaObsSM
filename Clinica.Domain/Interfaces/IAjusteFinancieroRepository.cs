using Clinica.Domain.Entities;

namespace Clinica.Domain.Interfaces;

public interface IAjusteFinancieroRepository : IGenericRepository<AjusteFinanciero>
{
    Task<IEnumerable<AjusteFinanciero>> ObtenerTodosConDetalleAsync();
    Task<IEnumerable<AjusteFinanciero>> ObtenerPorAtencionAsync(Guid atencionId);
    Task<IEnumerable<AjusteFinanciero>> ObtenerPorPagoAsync(Guid pagoId);
}