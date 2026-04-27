using System.Text.Json;
using Clinica.Domain.DTOs.Comprobantes;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class ComprobanteService : IComprobanteService
{
    private const decimal TasaIgv = 18m;

    private readonly IComprobanteRepository _comprobanteRepository;
    private readonly IPagoRepository _pagoRepository;
    private readonly IUsuarioActualService _usuarioActualService;
    private readonly IComprobantePdfService _comprobantePdfService;

    public ComprobanteService(
        IComprobanteRepository comprobanteRepository,
        IPagoRepository pagoRepository,
        IUsuarioActualService usuarioActualService,
        IComprobantePdfService comprobantePdfService)
    {
        _comprobanteRepository = comprobanteRepository;
        _pagoRepository = pagoRepository;
        _usuarioActualService = usuarioActualService;
        _comprobantePdfService = comprobantePdfService;
    }

    public async Task<Guid> EmitirComprobantePagoAsync(EmitirComprobantePagoDto dto)
    {
        if (dto.PagoId == Guid.Empty)
            throw new InvalidOperationException("El identificador del pago es obligatorio.");

        Pago? pago = null;

        if (!string.IsNullOrWhiteSpace(dto.CodigoPago))
        {
            pago = await _pagoRepository.ObtenerPorCodigoConDetalleAsync(dto.CodigoPago.Trim());
        }

        pago ??= await _pagoRepository.GetByIdAsync(dto.PagoId)
                 ?? throw new KeyNotFoundException("Pago no encontrado.");

        if (pago == null)
            pago = await _pagoRepository.GetByIdAsync(dto.PagoId)
                   ?? throw new KeyNotFoundException("Pago no encontrado.");

        var usuarioId = _usuarioActualService.ObtenerUsuarioId();

        var serie = ObtenerSerie(TipoComprobante.BoletaPago);
        var ultimoNumero = await _comprobanteRepository.ObtenerUltimoNumeroPorSerieAsync(serie);
        var numero = ultimoNumero + 1;

        var subtotal = CalcularSubtotalDesdeTotal(pago.MontoPagado, TasaIgv);
        var impuesto = pago.MontoPagado - subtotal;

        var comprobante = new Comprobante
        {
            Id = Guid.NewGuid(),
            CodigoComprobante = $"{serie}-{numero:000000}",
            Serie = serie,
            Numero = numero,
            TipoComprobante = TipoComprobante.BoletaPago,
            Estado = EstadoComprobante.Emitido,
            FormatoImpresion = TipoFormatoImpresion.A4,

            PacienteId = pago.PacienteId,
            PagoId = pago.Id,
            CitaId = pago.CitaId,
            AtencionId = pago.AtencionId,

            TipoDocumentoPaciente = TipoDocumentoComprobante.DNI,
            NumeroDocumentoPaciente = pago.Paciente?.DNI ?? "",
            NombrePaciente = pago.Paciente == null ? "" : $"{pago.Paciente.Nombres} {pago.Paciente.Apellidos}",
            DireccionPaciente = pago.Paciente?.Direccion,

            Subtotal = subtotal,
            TasaImpuesto = TasaIgv,
            MontoImpuesto = impuesto,
            Total = pago.MontoPagado,

            FechaEmision = DateTime.UtcNow,
            UsuarioEmisionId = usuarioId,
            Observacion = dto.Observacion,

            DatosSnapshotJson = JsonSerializer.Serialize(new
            {
                Pago = pago.CodigoPago,
                Paciente = pago.Paciente == null ? "" : $"{pago.Paciente.Nombres} {pago.Paciente.Apellidos}",
                pago.MontoTotal,
                pago.MontoPagado,
                pago.SaldoPendiente,
                pago.MetodoPago,
                pago.Estado
            })
        };

        comprobante.Detalles.Add(new ComprobanteDetalle
        {
            Id = Guid.NewGuid(),
            ComprobanteId = comprobante.Id,
            CodigoServicio = pago.ServicioClinico?.CodigoServicio ?? "",
            Descripcion = pago.ServicioClinico?.Nombre ?? "Servicio clínico",
            Cantidad = 1,
            PrecioUnitarioFinal = pago.MontoPagado,
            Subtotal = subtotal,
            TasaImpuesto = TasaIgv,
            MontoImpuesto = impuesto,
            Total = pago.MontoPagado
        });

        await _comprobanteRepository.AddAsync(comprobante);
        await _comprobanteRepository.SaveChangesAsync();

        return comprobante.Id;
    }

    public async Task<DocumentoGeneradoDto> GenerarPdfBoletaPagoAsync(Guid comprobanteId)
    {
        var comprobante = await _comprobanteRepository.ObtenerPorIdConDetalleAsync(comprobanteId)
            ?? throw new KeyNotFoundException("Comprobante no encontrado.");

        var preview = MapearPagoPreview(comprobante);
        var archivo = _comprobantePdfService.GenerarBoletaPagoPdf(preview);

        return new DocumentoGeneradoDto
        {
            NombreArchivo = $"{comprobante.CodigoComprobante}.pdf",
            ContentType = "application/pdf",
            Archivo = archivo
        };
    }

    public async Task<ComprobanteDto> ObtenerPorIdAsync(Guid id)
    {
        var comprobante = await _comprobanteRepository.ObtenerPorIdConDetalleAsync(id)
            ?? throw new KeyNotFoundException("Comprobante no encontrado.");

        return MapearComprobante(comprobante);
    }

    public async Task<IEnumerable<ComprobanteDto>> ObtenerPorPacienteAsync(Guid pacienteId)
    {
        var comprobantes = await _comprobanteRepository.ObtenerPorPacienteAsync(pacienteId);
        return comprobantes.Select(MapearComprobante).ToList();
    }

    public async Task<IEnumerable<ComprobanteDto>> ObtenerPorPagoAsync(Guid pagoId)
    {
        var comprobantes = await _comprobanteRepository.ObtenerPorPagoAsync(pagoId);
        return comprobantes.Select(MapearComprobante).ToList();
    }

    public async Task<IEnumerable<ComprobanteDto>> ObtenerPorAtencionAsync(Guid atencionId)
    {
        var comprobantes = await _comprobanteRepository.ObtenerPorAtencionAsync(atencionId);
        return comprobantes.Select(MapearComprobante).ToList();
    }

    public async Task AnularComprobanteAsync(Guid comprobanteId, string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new InvalidOperationException("El motivo de anulación es obligatorio.");

        var comprobante = await _comprobanteRepository.GetByIdAsync(comprobanteId)
            ?? throw new KeyNotFoundException("Comprobante no encontrado.");

        if (comprobante.Estado == EstadoComprobante.Anulado)
            throw new InvalidOperationException("El comprobante ya se encuentra anulado.");

        comprobante.Estado = EstadoComprobante.Anulado;
        comprobante.FechaAnulacion = DateTime.UtcNow;
        comprobante.UsuarioAnulacionId = _usuarioActualService.ObtenerUsuarioId();
        comprobante.MotivoAnulacion = motivo.Trim();

        await _comprobanteRepository.UpdateAsync(comprobante);
        await _comprobanteRepository.SaveChangesAsync();
    }

    private static string ObtenerSerie(TipoComprobante tipo)
    {
        return tipo switch
        {
            TipoComprobante.BoletaPago => "B001",
            TipoComprobante.ConstanciaCita => "C001",
            TipoComprobante.ResumenAtencion => "A001",
            TipoComprobante.EstadoCuenta => "E001",
            TipoComprobante.HistoriaClinica => "H001",
            _ => "D001"
        };
    }

    private static decimal CalcularSubtotalDesdeTotal(decimal total, decimal tasaImpuesto)
    {
        return Math.Round(total / (1 + tasaImpuesto / 100), 2);
    }

    private static ComprobantePagoPreviewDto MapearPagoPreview(Comprobante comprobante)
    {
        return new ComprobantePagoPreviewDto
        {
            CodigoComprobante = comprobante.CodigoComprobante,
            Paciente = comprobante.NombrePaciente,
            DniPaciente = comprobante.NumeroDocumentoPaciente,
            FechaEmision = comprobante.FechaEmision,
            Subtotal = comprobante.Subtotal,
            TasaImpuesto = comprobante.TasaImpuesto,
            MontoImpuesto = comprobante.MontoImpuesto,
            Total = comprobante.Total,
            Observacion = comprobante.Observacion,
            Detalles = comprobante.Detalles.Select(d => new ComprobanteDetalleDto
            {
                CodigoServicio = d.CodigoServicio,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                PrecioUnitarioFinal = d.PrecioUnitarioFinal,
                Subtotal = d.Subtotal,
                TasaImpuesto = d.TasaImpuesto,
                MontoImpuesto = d.MontoImpuesto,
                Total = d.Total
            }).ToList()
        };
    }

    private static ComprobanteDto MapearComprobante(Comprobante x)
    {
        return new ComprobanteDto
        {
            Id = x.Id,
            CodigoComprobante = x.CodigoComprobante,
            Serie = x.Serie,
            Numero = x.Numero,
            TipoComprobante = x.TipoComprobante.ToString(),
            Estado = x.Estado.ToString(),
            FormatoImpresion = x.FormatoImpresion.ToString(),
            PacienteId = x.PacienteId,
            Paciente = x.NombrePaciente,
            NumeroDocumentoPaciente = x.NumeroDocumentoPaciente,
            PagoId = x.PagoId,
            CitaId = x.CitaId,
            AtencionId = x.AtencionId,
            HistorialClinicoId = x.HistorialClinicoId,
            Subtotal = x.Subtotal,
            TasaImpuesto = x.TasaImpuesto,
            MontoImpuesto = x.MontoImpuesto,
            Total = x.Total,
            FechaEmision = x.FechaEmision,
            Observacion = x.Observacion,
            MotivoAnulacion = x.MotivoAnulacion,
            Detalles = x.Detalles.Select(d => new ComprobanteDetalleDto
            {
                CodigoServicio = d.CodigoServicio,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                PrecioUnitarioFinal = d.PrecioUnitarioFinal,
                Subtotal = d.Subtotal,
                TasaImpuesto = d.TasaImpuesto,
                MontoImpuesto = d.MontoImpuesto,
                Total = d.Total
            }).ToList()
        };
    }
}