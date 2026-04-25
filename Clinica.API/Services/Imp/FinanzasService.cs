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
            Pagos = pagosDelDia.Select(MapearPagoFinanzas).ToList()
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
                Pagos = g.Select(MapearPagoFinanzas).ToList()
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

    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPendientesAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas);
    }

    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosPagadosAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Pagado && x.SaldoPendiente <= 0)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas);
    }

    public async Task<IEnumerable<PagoFinanzasDto>> ObtenerPagosParcialesAsync()
    {
        var pagos = await ObtenerPagosValidosAsync();

        return pagos
            .Where(x => x.Estado == EstadoPago.Parcial)
            .OrderByDescending(x => x.FechaPago)
            .Select(MapearPagoFinanzas);
    }

    public async Task<PagoFinanzasDto?> ObtenerPagoPorCodigoAsync(string codigoPago)
    {
        if (string.IsNullOrWhiteSpace(codigoPago))
            throw new InvalidOperationException("El código de pago es obligatorio.");

        var pago = await _pagoRepository.ObtenerPorCodigoConDetalleAsync(codigoPago.Trim());

        return pago == null ? null : MapearPagoFinanzas(pago);
    }

    public async Task<EstadoCuentaPacienteDto> ObtenerEstadoCuentaPacienteAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId)
            ?? throw new KeyNotFoundException("Paciente no encontrado.");

        var pagos = await _pagoRepository.ObtenerPorPacienteAsync(pacienteId);

        var pagosValidos = pagos
            .Where(EsPagoValidoParaFinanzas)
            .OrderByDescending(x => x.FechaPago)
            .ToList();

        return new EstadoCuentaPacienteDto
        {
            PacienteId = paciente.Id,
            Paciente = $"{paciente.Nombres} {paciente.Apellidos}",
            DniPaciente = paciente.DNI,

            TotalFacturado = pagosValidos.Sum(x => x.MontoTotal),
            TotalPagado = pagosValidos.Sum(x => x.MontoPagado),
            TotalPendiente = pagosValidos.Sum(x => x.SaldoPendiente),

            CantidadPagos = pagosValidos.Count,
            PagosCompletados = pagosValidos.Count(x => x.Estado == EstadoPago.Pagado),
            PagosParciales = pagosValidos.Count(x => x.Estado == EstadoPago.Parcial),
            PagosPendientes = pagosValidos.Count(x => x.Estado == EstadoPago.Pendiente || x.SaldoPendiente > 0),

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

    private static void ValidarAnioMes(int anio, int mes)
    {
        if (anio < 2000 || anio > DateTime.UtcNow.Year + 1)
            throw new InvalidOperationException("El año ingresado no es válido.");

        if (mes < 1 || mes > 12)
            throw new InvalidOperationException("El mes ingresado no es válido.");
    }
}