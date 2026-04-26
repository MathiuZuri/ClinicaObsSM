using Clinica.Domain.DTOs.Finanzas;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class FinanzasService : IFinanzasService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly IPacienteRepository _pacienteRepository;

    public FinanzasService(
        IPagoRepository pagoRepository,
        IPacienteRepository pacienteRepository)
    {
        _pagoRepository = pagoRepository;
        _pacienteRepository = pacienteRepository;
    }

    // ==========================================================
    // DEUDAS REALES AGRUPADAS POR ATENCIÓN
    // ==========================================================

    public async Task<IEnumerable<EstadoPagoAtencionDto>> ObtenerDeudasRealesAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.AtencionId.HasValue)
            .GroupBy(x => x.AtencionId!.Value)
            .Select(MapearEstadoPagoAtencion)
            .Where(x => x.TieneDeuda)
            .OrderByDescending(x => x.SaldoReal)
            .ThenBy(x => x.Paciente)
            .ToList();
    }

    public async Task<IEnumerable<EstadoPagoAtencionDto>> ObtenerDeudasRealesPacienteAsync(Guid pacienteId)
    {
        if (pacienteId == Guid.Empty)
            throw new InvalidOperationException("El identificador del paciente es obligatorio.");

        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId)
            ?? throw new KeyNotFoundException("Paciente no encontrado.");

        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.PacienteId == paciente.Id && x.AtencionId.HasValue)
            .GroupBy(x => x.AtencionId!.Value)
            .Select(MapearEstadoPagoAtencion)
            .Where(x => x.TieneDeuda)
            .OrderByDescending(x => x.SaldoReal)
            .ToList();
    }

    public async Task<EstadoPagoAtencionDto> ObtenerEstadoPagoAtencionAsync(Guid atencionId)
    {
        if (atencionId == Guid.Empty)
            throw new InvalidOperationException("El identificador de la atención es obligatorio.");

        var pagos = await ObtenerPagosValidosAsync();

        var pagosAtencion = pagos
            .Where(x => x.AtencionId == atencionId)
            .ToList();

        if (!pagosAtencion.Any())
            throw new KeyNotFoundException("No se encontraron pagos asociados a la atención.");

        return pagosAtencion
            .GroupBy(x => x.AtencionId!.Value)
            .Select(MapearEstadoPagoAtencion)
            .First();
    }

    // ==========================================================
    // RESÚMENES DE CAJA / MOVIMIENTOS INDIVIDUALES
    // ==========================================================

    public async Task<ResumenDiarioFinanzasDto> ObtenerResumenDiarioAsync(DateOnly fecha)
    {
        var pagos = await ObtenerPagosValidosAsync();

        var pagosDelDia = pagos
            .Where(x => DateOnly.FromDateTime(x.FechaPago) == fecha)
            .ToList();

        return new ResumenDiarioFinanzasDto
        {
            Fecha = fecha,
            TotalIngresos = pagosDelDia.Sum(x => x.MontoPagado),
            TotalPendiente = pagosDelDia.Sum(x => x.SaldoPendiente),
            TotalDeuda = pagosDelDia.Where(x => x.SaldoPendiente > 0).Sum(x => x.SaldoPendiente),
            CantidadPagos = pagosDelDia.Count,
            PagosCompletados = pagosDelDia.Count(x => x.Estado == EstadoPago.Pagado),
            PagosParciales = pagosDelDia.Count(x => x.Estado == EstadoPago.Parcial),
            PagosPendientes = pagosDelDia.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0),
            Pagos = pagosDelDia
                .OrderByDescending(x => x.FechaPago)
                .Select(MapearPagoFinanzas)
                .ToList()
        };
    }

    public async Task<ResumenMensualFinanzasDto> ObtenerResumenMensualAsync(int anio, int mes)
    {
        ValidarAnioMes(anio, mes);

        var pagos = await ObtenerPagosValidosAsync();

        var pagosDelMes = pagos
            .Where(x => x.FechaPago.Year == anio && x.FechaPago.Month == mes)
            .ToList();

        var dias = pagosDelMes
            .GroupBy(x => DateOnly.FromDateTime(x.FechaPago))
            .OrderBy(x => x.Key)
            .Select(g => new ResumenDiarioFinanzasDto
            {
                Fecha = g.Key,
                TotalIngresos = g.Sum(x => x.MontoPagado),
                TotalPendiente = g.Sum(x => x.SaldoPendiente),
                TotalDeuda = g.Where(x => x.SaldoPendiente > 0).Sum(x => x.SaldoPendiente),
                CantidadPagos = g.Count(),
                PagosCompletados = g.Count(x => x.Estado == EstadoPago.Pagado),
                PagosParciales = g.Count(x => x.Estado == EstadoPago.Parcial),
                PagosPendientes = g.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0),
                Pagos = g
                    .OrderByDescending(x => x.FechaPago)
                    .Select(MapearPagoFinanzas)
                    .ToList()
            })
            .ToList();

        return new ResumenMensualFinanzasDto
        {
            Anio = anio,
            Mes = mes,
            TotalIngresos = pagosDelMes.Sum(x => x.MontoPagado),
            TotalPendiente = pagosDelMes.Sum(x => x.SaldoPendiente),
            TotalDeuda = pagosDelMes.Where(x => x.SaldoPendiente > 0).Sum(x => x.SaldoPendiente),
            CantidadPagos = pagosDelMes.Count,
            PagosCompletados = pagosDelMes.Count(x => x.Estado == EstadoPago.Pagado),
            PagosParciales = pagosDelMes.Count(x => x.Estado == EstadoPago.Parcial),
            PagosPendientes = pagosDelMes.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0),
            Dias = dias
        };
    }

    public async Task<ResumenAnualFinanzasDto> ObtenerResumenAnualAsync(int anio)
    {
        if (anio < 2000 || anio > DateTime.UtcNow.Year + 1)
            throw new InvalidOperationException("El año ingresado no es válido.");

        var pagos = await ObtenerPagosValidosAsync();

        var pagosDelAnio = pagos
            .Where(x => x.FechaPago.Year == anio)
            .ToList();

        var meses = Enumerable.Range(1, 12)
            .Select(mes =>
            {
                var pagosMes = pagosDelAnio
                    .Where(x => x.FechaPago.Month == mes)
                    .ToList();

                return new ResumenMensualFinanzasDto
                {
                    Anio = anio,
                    Mes = mes,
                    TotalIngresos = pagosMes.Sum(x => x.MontoPagado),
                    TotalPendiente = pagosMes.Sum(x => x.SaldoPendiente),
                    TotalDeuda = pagosMes.Where(x => x.SaldoPendiente > 0).Sum(x => x.SaldoPendiente),
                    CantidadPagos = pagosMes.Count,
                    PagosCompletados = pagosMes.Count(x => x.Estado == EstadoPago.Pagado),
                    PagosParciales = pagosMes.Count(x => x.Estado == EstadoPago.Parcial),
                    PagosPendientes = pagosMes.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0)
                };
            })
            .ToList();

        return new ResumenAnualFinanzasDto
        {
            Anio = anio,
            TotalIngresos = pagosDelAnio.Sum(x => x.MontoPagado),
            TotalPendiente = pagosDelAnio.Sum(x => x.SaldoPendiente),
            TotalDeuda = pagosDelAnio.Where(x => x.SaldoPendiente > 0).Sum(x => x.SaldoPendiente),
            CantidadPagos = pagosDelAnio.Count,
            PagosCompletados = pagosDelAnio.Count(x => x.Estado == EstadoPago.Pagado),
            PagosParciales = pagosDelAnio.Count(x => x.Estado == EstadoPago.Parcial),
            PagosPendientes = pagosDelAnio.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0),
            Meses = meses
        };
    }

    // Lista movimientos de pago individuales con saldo pendiente.
    // Para deuda real agrupada por atención usar ObtenerDeudasRealesAsync().
    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPendientesAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas)
            .ToList();
    }

    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPagadosAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Pagado && x.SaldoPendiente <= 0)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas)
            .ToList();
    }

    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosParcialesAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Parcial)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas)
            .ToList();
    }

    public async Task<PagoFinanzasDto?> ObtenerPagoPorCodigoAsync(string codigoPago)
    {
        if (string.IsNullOrWhiteSpace(codigoPago))
            throw new InvalidOperationException("El código de pago es obligatorio.");

        var pago = await _pagoRepository.ObtenerPorCodigoConDetalleAsync(codigoPago.Trim());

        return pago == null ? null : MapearPagoFinanzas(pago);
    }

    // ==========================================================
    // ESTADO DE CUENTA DEL PACIENTE
    // ==========================================================

    public async Task<EstadoCuentaPacienteDto> ObtenerEstadoCuentaPacienteAsync(Guid pacienteId)
    {
        if (pacienteId == Guid.Empty)
            throw new InvalidOperationException("El identificador del paciente es obligatorio.");

        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId)
            ?? throw new KeyNotFoundException("Paciente no encontrado.");

        var pagos = await _pagoRepository.ObtenerPorPacienteAsync(pacienteId);

        var pagosValidos = pagos
            .Where(EsPagoValidoParaFinanzas)
            .OrderByDescending(x => x.FechaPago)
            .ToList();

        var estadosPorAtencion = pagosValidos
            .Where(x => x.AtencionId.HasValue)
            .GroupBy(x => x.AtencionId!.Value)
            .Select(MapearEstadoPagoAtencion)
            .ToList();

        return new EstadoCuentaPacienteDto
        {
            PacienteId = paciente.Id,
            Paciente = $"{paciente.Nombres} {paciente.Apellidos}",
            DniPaciente = paciente.DNI,

            // Cálculo profesional: no duplica MontoTotal si hay varios pagos de una misma atención.
            TotalFacturado = estadosPorAtencion.Sum(x => x.MontoTotal),
            TotalPagado = estadosPorAtencion.Sum(x => x.TotalPagado),
            TotalPendiente = estadosPorAtencion.Sum(x => x.SaldoReal),

            // CantidadPagos representa movimientos individuales.
            CantidadPagos = pagosValidos.Count,

            // Estos contadores representan estado real agrupado por atención.
            PagosCompletados = estadosPorAtencion.Count(x => x.EstadoFinanciero == "Pagado"),
            PagosParciales = estadosPorAtencion.Count(x => x.EstadoFinanciero == "Parcial"),
            PagosPendientes = estadosPorAtencion.Count(x => x.EstadoFinanciero == "Pendiente" || x.TieneDeuda),

            // Detalles mantiene los movimientos individuales para trazabilidad y reclamos.
            Detalles = pagosValidos.Select(x => new DetalleEstadoCuentaDto
            {
                PagoId = x.Id,
                CodigoPago = x.CodigoPago,
                AtencionId = x.AtencionId,
                Servicio = x.ServicioClinico?.Nombre ?? "",
                MontoTotal = x.MontoTotal,
                MontoPagado = x.MontoPagado,
                SaldoPendiente = x.SaldoPendiente,
                EstadoPago = x.Estado.ToString(),
                FechaPago = x.FechaPago
            }).ToList()
        };
    }

    // ==========================================================
    // MÉTODOS PRIVADOS
    // ==========================================================

    private async Task<List<Pago>> ObtenerPagosValidosAsync()
    {
        var pagos = await _pagoRepository.ObtenerTodosConDetalleAsync();

        return pagos
            .Where(EsPagoValidoParaFinanzas)
            .ToList();
    }

    private static bool EsPagoValidoParaFinanzas(Pago pago)
    {
        return pago.Estado != EstadoPago.Anulado
               && pago.Estado != EstadoPago.Reembolsado
               && pago.Estado != EstadoPago.Eliminado;
    }

    private static PagoFinanzasDto MapearPagoFinanzas(Pago x)
    {
        return new PagoFinanzasDto
        {
            PagoId = x.Id,
            CodigoPago = x.CodigoPago,

            PacienteId = x.PacienteId,
            Paciente = x.Paciente == null ? "" : $"{x.Paciente.Nombres} {x.Paciente.Apellidos}",
            DniPaciente = x.Paciente?.DNI ?? "",

            AtencionId = x.AtencionId,
            Servicio = x.ServicioClinico?.Nombre ?? "",

            MontoTotal = x.MontoTotal,
            MontoPagado = x.MontoPagado,
            SaldoPendiente = x.SaldoPendiente,

            EstadoPago = x.Estado.ToString(),
            MetodoPago = x.MetodoPago.ToString(),

            FechaPago = x.FechaPago,
            RegistradoPor = x.UsuarioRegistro == null
                ? ""
                : $"{x.UsuarioRegistro.Nombres} {x.UsuarioRegistro.Apellidos}"
        };
    }

    private static EstadoPagoAtencionDto MapearEstadoPagoAtencion(IGrouping<Guid, Pago> grupo)
    {
        var pagos = grupo
            .OrderBy(x => x.FechaPago)
            .ToList();

        var primerPago = pagos.First();

        /*
         * IMPORTANTE:
         * No se suma MontoTotal, porque una misma atención puede tener varios pagos.
         * Ejemplo:
         * Atención: S/ 70
         * Pago 1: S/ 10, MontoTotal = 70
         * Pago 2: S/ 70, MontoTotal = 70
         *
         * Si sumamos MontoTotal sería 140, lo cual es incorrecto.
         * Por eso se toma el monto mayor registrado para la atención.
         */
        var montoTotal = pagos.Max(x => x.MontoTotal);
        var totalPagado = pagos.Sum(x => x.MontoPagado);

        var saldoReal = Math.Max(montoTotal - totalPagado, 0);
        var sobrepago = Math.Max(totalPagado - montoTotal, 0);

        string estadoFinanciero;

        if (sobrepago > 0)
            estadoFinanciero = "Sobrepagado";
        else if (saldoReal == 0)
            estadoFinanciero = "Pagado";
        else if (totalPagado > 0)
            estadoFinanciero = "Parcial";
        else
            estadoFinanciero = "Pendiente";

        return new EstadoPagoAtencionDto
        {
            AtencionId = grupo.Key,

            PacienteId = primerPago.PacienteId,
            Paciente = primerPago.Paciente == null
                ? ""
                : $"{primerPago.Paciente.Nombres} {primerPago.Paciente.Apellidos}",
            DniPaciente = primerPago.Paciente?.DNI ?? "",

            Servicio = primerPago.ServicioClinico?.Nombre ?? "",

            MontoTotal = montoTotal,
            TotalPagado = totalPagado,
            SaldoReal = saldoReal,
            Sobrepago = sobrepago,

            TieneDeuda = saldoReal > 0,
            TieneSobrepago = sobrepago > 0,

            EstadoFinanciero = estadoFinanciero,

            FechaPrimerPago = pagos.Min(x => x.FechaPago),
            FechaUltimoPago = pagos.Max(x => x.FechaPago),

            CantidadPagos = pagos.Count,
            Pagos = pagos
                .OrderByDescending(x => x.FechaPago)
                .Select(MapearPagoFinanzas)
                .ToList()
        };
    }

    private static void ValidarAnioMes(int anio, int mes)
    {
        if (anio < 2000 || anio > DateTime.UtcNow.Year + 1)
            throw new InvalidOperationException("El año ingresado no es válido.");

        if (mes < 1 || mes > 12)
            throw new InvalidOperationException("El mes ingresado no es válido.");
    }
}