using Clinica.Domain.DTOs.Comprobantes;

namespace Clinica.Domain.Interfaces;

public interface IComprobantePdfService
{
    Task<DocumentoGeneradoDto> GenerarBoletaPagoPdfAsync(Guid comprobanteId);
    Task<DocumentoGeneradoDto> GenerarConstanciaCitaPdfAsync(Guid comprobanteId);
    Task<DocumentoGeneradoDto> GenerarResumenAtencionPdfAsync(Guid comprobanteId);
    Task<DocumentoGeneradoDto> GenerarEstadoCuentaPacientePdfAsync(Guid pacienteId);
    Task<DocumentoGeneradoDto> GenerarHistoriaClinicaPdfAsync(Guid historialClinicoId);
    Task<DocumentoGeneradoDto> GenerarReporteCajaDiarioPdfAsync(DateOnly fecha);
    Task<DocumentoGeneradoDto> GenerarReporteFinancieroMensualPdfAsync(int anio, int mes);
}