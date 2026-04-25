using Clinica.Domain.DTOs.Finanzas;

namespace Clinica.Domain.Interfaces;

public interface IFinanzasService
{
    Task<ResumenDiarioFinanzasDto> ObtenerResumenDiarioAsync(DateOnly fecha);
    Task<ResumenMensualFinanzasDto> ObtenerResumenMensualAsync(int anio, int mes);
    Task<ResumenAnualFinanzasDto> ObtenerResumenAnualAsync(int anio);

    Task<List<PagoFinanzasDto>> ObtenerPagosPendientesAsync();
    Task<List<PagoFinanzasDto>> ObtenerPagosPagadosAsync();
    Task<List<PagoFinanzasDto>> ObtenerPagosParcialesAsync();

    Task<PagoFinanzasDto> BuscarPagoPorCodigoAsync(string codigoPago);
    Task<EstadoCuentaPacienteDto> ObtenerEstadoCuentaPacienteAsync(Guid pacienteId);
}