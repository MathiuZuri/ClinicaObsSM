using Clinica.Domain.Entities;
using Clinica.Domain.Enums;

namespace Clinica.Domain.Interfaces;

public interface IComprobanteRepository : IGenericRepository<Comprobante>
{
    Task<Comprobante?> ObtenerPorIdConDetalleAsync(Guid id);

    Task<IEnumerable<Comprobante>> ObtenerPorPacienteAsync(Guid pacienteId);
    Task<IEnumerable<Comprobante>> ObtenerPorPagoAsync(Guid pagoId);
    Task<IEnumerable<Comprobante>> ObtenerPorCitaAsync(Guid citaId);
    Task<IEnumerable<Comprobante>> ObtenerPorAtencionAsync(Guid atencionId);
    Task<IEnumerable<Comprobante>> ObtenerPorHistorialClinicoAsync(Guid historialClinicoId);

    Task<Comprobante?> ObtenerUltimoPorSerieAsync(string serie, TipoComprobante tipoComprobante);

    Task<bool> ExisteComprobanteActivoPorPagoAsync(Guid pagoId, TipoComprobante tipoComprobante);
    Task<bool> ExisteComprobanteActivoPorCitaAsync(Guid citaId, TipoComprobante tipoComprobante);
    Task<bool> ExisteComprobanteActivoPorAtencionAsync(Guid atencionId, TipoComprobante tipoComprobante);
    Task<bool> ExisteComprobanteActivoPorHistorialClinicoAsync(Guid historialClinicoId, TipoComprobante tipoComprobante);
}