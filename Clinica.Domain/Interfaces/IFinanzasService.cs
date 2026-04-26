using Clinica.Domain.DTOs.Finanzas;

namespace Clinica.Domain.Interfaces;

public interface IFinanzasService
{
    Task<ResumenDiarioFinanzasDto> ObtenerResumenDiarioAsync(DateOnly fecha);
    Task<ResumenMensualFinanzasDto> ObtenerResumenMensualAsync(int anio, int mes);
    Task<ResumenAnualFinanzasDto> ObtenerResumenAnualAsync(int anio);

    Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPendientesAsync();
    Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPagadosAsync();
    Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosParcialesAsync();

    Task<PagoFinanzasDto?> ObtenerPagoPorCodigoAsync(string codigoPago);
    Task<EstadoCuentaPacienteDto> ObtenerEstadoCuentaPacienteAsync(Guid pacienteId);
}