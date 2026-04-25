using Clinica.Domain.DTOs.Finanzas;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clinica.API.Services.Imp;

public class FinanzasService : IFinanzasService
{
    private readonly ApplicationDbContext _context;

    public FinanzasService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumenDiarioFinanzasDto> ObtenerResumenDiarioAsync(DateOnly fecha)
    {
        var inicio = fecha.ToDateTime(TimeOnly.MinValue);
        var fin = fecha.ToDateTime(TimeOnly.MaxValue);

        var pagos = await ObtenerPagosBase()
            .Where(p => p.FechaPago >= inicio && p.FechaPago <= fin)
            .ToListAsync();

        return new ResumenDiarioFinanzasDto
        {
            Fecha = fecha,
            TotalIngresos = pagos.Sum(p => p.MontoPagado),
            TotalPendiente = pagos.Sum(p => p.SaldoPendiente),
            TotalDeuda = pagos.Where(p => p.SaldoPendiente > 0).Sum(p => p.SaldoPendiente),
            CantidadPagos = pagos.Count,
            PagosCompletados = pagos.Count(p => p.EstadoPago == "Pagado"),
            PagosParciales = pagos.Count(p => p.EstadoPago == "Parcial"),
            PagosPendientes = pagos.Count(p => p.EstadoPago == "Pendiente"),
            Pagos = pagos
        };
    }

    public async Task<ResumenMensualFinanzasDto> ObtenerResumenMensualAsync(int anio, int mes)
    {
        if (mes < 1 || mes > 12)
            throw new Exception("El mes debe estar entre 1 y 12.");

        var inicio = new DateTime(anio, mes, 1);
        var fin = inicio.AddMonths(1).AddTicks(-1);

        var pagos = await ObtenerPagosBase()
            .Where(p => p.FechaPago >= inicio && p.FechaPago <= fin)
            .ToListAsync();

        var dias = pagos
            .GroupBy(p => DateOnly.FromDateTime(p.FechaPago))
            .Select(g => new ResumenDiarioFinanzasDto
            {
                Fecha = g.Key,
                TotalIngresos = g.Sum(p => p.MontoPagado),
                TotalPendiente = g.Sum(p => p.SaldoPendiente),
                TotalDeuda = g.Where(p => p.SaldoPendiente > 0).Sum(p => p.SaldoPendiente),
                CantidadPagos = g.Count(),
                PagosCompletados = g.Count(p => p.EstadoPago == "Pagado"),
                PagosParciales = g.Count(p => p.EstadoPago == "Parcial"),
                PagosPendientes = g.Count(p => p.EstadoPago == "Pendiente"),
                Pagos = g.ToList()
            })
            .OrderBy(d => d.Fecha)
            .ToList();

        return new ResumenMensualFinanzasDto
        {
            Anio = anio,
            Mes = mes,
            TotalIngresos = pagos.Sum(p => p.MontoPagado),
            TotalPendiente = pagos.Sum(p => p.SaldoPendiente),
            TotalDeuda = pagos.Where(p => p.SaldoPendiente > 0).Sum(p => p.SaldoPendiente),
            CantidadPagos = pagos.Count,
            PagosCompletados = pagos.Count(p => p.EstadoPago == "Pagado"),
            PagosParciales = pagos.Count(p => p.EstadoPago == "Parcial"),
            PagosPendientes = pagos.Count(p => p.EstadoPago == "Pendiente"),
            Dias = dias
        };
    }

    public async Task<ResumenAnualFinanzasDto> ObtenerResumenAnualAsync(int anio)
    {
        var inicio = new DateTime(anio, 1, 1);
        var fin = new DateTime(anio, 12, 31, 23, 59, 59);

        var pagos = await ObtenerPagosBase()
            .Where(p => p.FechaPago >= inicio && p.FechaPago <= fin)
            .ToListAsync();

        var meses = pagos
            .GroupBy(p => p.FechaPago.Month)
            .Select(g => new ResumenMensualFinanzasDto
            {
                Anio = anio,
                Mes = g.Key,
                TotalIngresos = g.Sum(p => p.MontoPagado),
                TotalPendiente = g.Sum(p => p.SaldoPendiente),
                TotalDeuda = g.Where(p => p.SaldoPendiente > 0).Sum(p => p.SaldoPendiente),
                CantidadPagos = g.Count(),
                PagosCompletados = g.Count(p => p.EstadoPago == "Pagado"),
                PagosParciales = g.Count(p => p.EstadoPago == "Parcial"),
                PagosPendientes = g.Count(p => p.EstadoPago == "Pendiente")
            })
            .OrderBy(m => m.Mes)
            .ToList();

        return new ResumenAnualFinanzasDto
        {
            Anio = anio,
            TotalIngresos = pagos.Sum(p => p.MontoPagado),
            TotalPendiente = pagos.Sum(p => p.SaldoPendiente),
            TotalDeuda = pagos.Where(p => p.SaldoPendiente > 0).Sum(p => p.SaldoPendiente),
            CantidadPagos = pagos.Count,
            PagosCompletados = pagos.Count(p => p.EstadoPago == "Pagado"),
            PagosParciales = pagos.Count(p => p.EstadoPago == "Parcial"),
            PagosPendientes = pagos.Count(p => p.EstadoPago == "Pendiente"),
            Meses = meses
        };
    }

    public async Task<List<PagoFinanzasDto>> ObtenerPagosPendientesAsync()
    {
        return await ObtenerPagosBase()
            .Where(p => p.EstadoPago == "Pendiente")
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<List<PagoFinanzasDto>> ObtenerPagosPagadosAsync()
    {
        return await ObtenerPagosBase()
            .Where(p => p.EstadoPago == "Pagado")
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<List<PagoFinanzasDto>> ObtenerPagosParcialesAsync()
    {
        return await ObtenerPagosBase()
            .Where(p => p.EstadoPago == "Parcial")
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<PagoFinanzasDto> BuscarPagoPorCodigoAsync(string codigoPago)
    {
        if (string.IsNullOrWhiteSpace(codigoPago))
            throw new Exception("Debe ingresar un código de pago válido.");

        var pago = await ObtenerPagosBase()
            .FirstOrDefaultAsync(p => p.CodigoPago == codigoPago);

        if (pago is null)
            throw new Exception("No se encontró ningún pago con el código ingresado.");

        return pago;
    }

    public async Task<EstadoCuentaPacienteDto> ObtenerEstadoCuentaPacienteAsync(Guid pacienteId)
    {
        if (pacienteId == Guid.Empty)
            throw new Exception("El ID del paciente no es válido.");

        var paciente = await _context.Pacientes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == pacienteId);

        if (paciente is null)
            throw new Exception("El paciente no existe.");

        var pagos = await ObtenerPagosBase()
            .Where(p => p.PacienteId == pacienteId)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();

        return new EstadoCuentaPacienteDto
        {
            PacienteId = paciente.Id,
            Paciente = $"{paciente.Nombres} {paciente.Apellidos}",
            DniPaciente = paciente.Dni,

            TotalFacturado = pagos.Sum(p => p.MontoTotal),
            TotalPagado = pagos.Sum(p => p.MontoPagado),
            TotalPendiente = pagos.Sum(p => p.SaldoPendiente),

            CantidadPagos = pagos.Count,
            PagosCompletados = pagos.Count(p => p.EstadoPago == "Pagado"),
            PagosParciales = pagos.Count(p => p.EstadoPago == "Parcial"),
            PagosPendientes = pagos.Count(p => p.EstadoPago == "Pendiente"),

            Detalles = pagos.Select(p => new DetalleEstadoCuentaDto
            {
                PagoId = p.PagoId,
                CodigoPago = p.CodigoPago,
                AtencionId = p.AtencionId,
                Servicio = p.Servicio,
                MontoTotal = p.MontoTotal,
                MontoPagado = p.MontoPagado,
                SaldoPendiente = p.SaldoPendiente,
                EstadoPago = p.EstadoPago,
                FechaPago = p.FechaPago
            }).ToList()
        };
    }

    private IQueryable<PagoFinanzasDto> ObtenerPagosBase()
    {
        return _context.Pagos
            .AsNoTracking()
            .Select(p => new PagoFinanzasDto
            {
                PagoId = p.Id,
                CodigoPago = p.CodigoPago,

                PacienteId = p.Atencion != null ? p.Atencion.PacienteId : null,
                Paciente = p.Atencion != null
                    ? p.Atencion.Paciente.Nombres + " " + p.Atencion.Paciente.Apellidos
                    : string.Empty,
                DniPaciente = p.Atencion != null
                    ? p.Atencion.Paciente.Dni
                    : string.Empty,

                AtencionId = p.AtencionId,
                Servicio = p.Atencion != null && p.Atencion.ServicioClinico != null
                    ? p.Atencion.ServicioClinico.Nombre
                    : "Sin servicio",

                MontoTotal = p.MontoTotal,
                MontoPagado = p.MontoPagado,
                SaldoPendiente = p.MontoTotal - p.MontoPagado,

                EstadoPago = p.EstadoPago,
                MetodoPago = p.MetodoPago,
                FechaPago = p.FechaPago,

                RegistradoPor = p.UsuarioRegistro != null
                    ? p.UsuarioRegistro.NombreUsuario
                    : "Sistema"
            });
    }
}