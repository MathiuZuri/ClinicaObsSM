using Clinica.Domain.DTOs.Comprobantes;

namespace Clinica.Domain.Interfaces;

public interface IComprobanteService
{
    Task<ComprobantePagoPreviewDto> PreviewBoletaPagoAsync(Guid pagoId, decimal tasaImpuesto = 0.18m);
    Task<Guid> EmitirBoletaPagoAsync(EmitirComprobantePagoDto dto);

    Task<ComprobanteCitaPreviewDto> PreviewConstanciaCitaAsync(Guid citaId);
    Task<Guid> EmitirConstanciaCitaAsync(EmitirComprobanteCitaDto dto);

    Task<ComprobanteAtencionPreviewDto> PreviewResumenAtencionAsync(Guid atencionId);
    Task<Guid> EmitirResumenAtencionAsync(EmitirComprobanteAtencionDto dto);

    Task<ComprobanteEstadoCuentaPreviewDto> PreviewEstadoCuentaPacienteAsync(Guid pacienteId);

    Task<ComprobanteHistoriaClinicaPreviewDto> PreviewHistoriaClinicaAsync(Guid historialClinicoId);
    Task<Guid> EmitirHistoriaClinicaAsync(Guid historialClinicoId);

    Task<ComprobanteDto> ObtenerComprobanteAsync(Guid comprobanteId);

    Task<IEnumerable<ComprobanteDto>> ObtenerComprobantesPorPacienteAsync(Guid pacienteId);
    Task<IEnumerable<ComprobanteDto>> ObtenerComprobantesPorPagoAsync(Guid pagoId);
    Task<IEnumerable<ComprobanteDto>> ObtenerComprobantesPorAtencionAsync(Guid atencionId);

    Task AnularComprobanteAsync(Guid comprobanteId, string motivo);
}