using Clinica.Domain.DTOs.Citas;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IServicioClinicoRepository _servicioRepository;
    private readonly IHistorialClinicoRepository _historialRepository;
    private readonly IHistorialDetalleRepository _detalleRepository;

    public CitaService(
        ICitaRepository citaRepository,
        IPacienteRepository pacienteRepository,
        IDoctorRepository doctorRepository,
        IServicioClinicoRepository servicioRepository,
        IHistorialClinicoRepository historialRepository,
        IHistorialDetalleRepository detalleRepository)
    {
        _citaRepository = citaRepository;
        _pacienteRepository = pacienteRepository;
        _doctorRepository = doctorRepository;
        _servicioRepository = servicioRepository;
        _historialRepository = historialRepository;
        _detalleRepository = detalleRepository;
    }

    public async Task<IEnumerable<CitaResponseDto>> ObtenerTodasAsync()
    {
        var citas = await _citaRepository.GetAllAsync();
        return citas.Select(MapearCita);
    }

    public async Task<CitaResponseDto?> ObtenerPorIdAsync(Guid id)
    {
        var cita = await _citaRepository.GetByIdAsync(id);
        if (cita == null) return null;

        return MapearCita(cita);
    }

    public async Task<IEnumerable<CitaResponseDto>> ObtenerPorPacienteAsync(Guid pacienteId)
    {
        var citas = await _citaRepository.ObtenerPorPacienteAsync(pacienteId);
        return citas.Select(MapearCita);
    }

    public async Task<IEnumerable<CitaResponseDto>> ObtenerPorDoctorAsync(Guid doctorId)
    {
        var citas = await _citaRepository.ObtenerPorDoctorAsync(doctorId);
        return citas.Select(MapearCita);
    }

    public async Task<Guid> CrearAsync(CrearCitaDto dto)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(dto.PacienteId)
            ?? throw new KeyNotFoundException("Paciente no encontrado.");

        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId)
            ?? throw new KeyNotFoundException("Doctor no encontrado.");

        var servicio = await _servicioRepository.GetByIdAsync(dto.ServicioClinicoId)
            ?? throw new KeyNotFoundException("Servicio clínico no encontrado.");

        var existeCruce = await _citaRepository.ExisteInterferenciaHorarioAsync(
            dto.DoctorId,
            dto.Fecha,
            dto.HoraInicio,
            dto.HoraFin);

        if (existeCruce)
            throw new InvalidOperationException("El doctor ya tiene una cita en ese horario.");

        var cita = new Cita
        {
            Id = Guid.NewGuid(),
            CodigoCita = GenerarCodigo("CIT", paciente.DNI),
            PacienteId = dto.PacienteId,
            DoctorId = dto.DoctorId,
            ServicioClinicoId = dto.ServicioClinicoId,
            HorarioDoctorId = dto.HorarioDoctorId,
            Fecha = dto.Fecha,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            Motivo = dto.Motivo,
            Observaciones = dto.Observaciones,
            Estado = EstadoCita.Pendiente,
            FechaRegistro = DateTime.UtcNow,
            UsuarioRegistroId = dto.UsuarioRegistroId
        };

        await _citaRepository.AddAsync(cita);

        var historial = await _historialRepository.ObtenerPorPacienteAsync(dto.PacienteId);
        if (historial != null)
        {
            await _detalleRepository.AddAsync(new HistorialDetalle
            {
                Id = Guid.NewGuid(),
                CodigoDetalle = GenerarCodigo(servicio.CodigoServicio, paciente.DNI),
                HistorialClinicoId = historial.Id,
                CitaId = cita.Id,
                TipoMovimiento = TipoMovimientoHistorial.CitaProgramada,
                Titulo = "Cita programada",
                Descripcion = $"Se programó una cita para el servicio {servicio.Nombre}.",
                FechaRegistro = DateTime.UtcNow,
                UsuarioId = dto.UsuarioRegistroId
            });
        }

        await _citaRepository.SaveChangesAsync();
        return cita.Id;
    }

    public async Task ReprogramarAsync(Guid id, ReprogramarCitaDto dto)
    {
        var cita = await _citaRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Cita no encontrada.");

        var existeCruce = await _citaRepository.ExisteInterferenciaHorarioAsync(
            dto.DoctorId,
            dto.NuevaFecha,
            dto.NuevaHoraInicio,
            dto.NuevaHoraFin,
            id);

        if (existeCruce)
            throw new InvalidOperationException("El doctor ya tiene una cita en ese nuevo horario.");

        cita.DoctorId = dto.DoctorId;
        cita.HorarioDoctorId = dto.HorarioDoctorId;
        cita.Fecha = dto.NuevaFecha;
        cita.HoraInicio = dto.NuevaHoraInicio;
        cita.HoraFin = dto.NuevaHoraFin;
        cita.Estado = EstadoCita.Reprogramada;
        cita.Observaciones = dto.MotivoReprogramacion;

        _citaRepository.Update(cita);
        await _citaRepository.SaveChangesAsync();
    }

    public async Task CancelarAsync(Guid id, CancelarCitaDto dto)
    {
        var cita = await _citaRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Cita no encontrada.");

        cita.Estado = EstadoCita.Cancelada;
        cita.Observaciones = dto.MotivoCancelacion;

        _citaRepository.Update(cita);
        await _citaRepository.SaveChangesAsync();
    }

    private static CitaResponseDto MapearCita(Cita cita)
    {
        return new CitaResponseDto
        {
            Id = cita.Id,
            CodigoCita = cita.CodigoCita,
            PacienteId = cita.PacienteId,
            PacienteNombre = cita.Paciente == null ? string.Empty : $"{cita.Paciente.Nombres} {cita.Paciente.Apellidos}",
            DoctorId = cita.DoctorId,
            DoctorNombre = cita.Doctor == null ? string.Empty : $"{cita.Doctor.Nombres} {cita.Doctor.Apellidos}",
            ServicioClinicoId = cita.ServicioClinicoId,
            ServicioNombre = cita.ServicioClinico?.Nombre ?? string.Empty,
            Fecha = cita.Fecha,
            HoraInicio = cita.HoraInicio,
            HoraFin = cita.HoraFin,
            Motivo = cita.Motivo,
            Observaciones = cita.Observaciones,
            Estado = cita.Estado,
            FechaRegistro = cita.FechaRegistro
        };
    }

    private static string GenerarCodigo(string prefijo, string dni)
    {
        return $"{Guid.NewGuid().ToString("N")[..5].ToUpper()}-{prefijo}-{DateTime.UtcNow:yyyy}-{dni}";
    }
}