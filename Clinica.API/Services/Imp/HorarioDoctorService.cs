using Clinica.Domain.DTOs.Horarios;
using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class HorarioDoctorService : IHorarioDoctorService
{
    private readonly IHorarioDoctorRepository _horarioRepository;
    private readonly IDoctorRepository _doctorRepository;

    public HorarioDoctorService(
        IHorarioDoctorRepository horarioRepository,
        IDoctorRepository doctorRepository)
    {
        _horarioRepository = horarioRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<HorarioDoctorResponseDto>> ObtenerTodosAsync()
    {
        var horarios = await _horarioRepository.GetAllAsync();

        return horarios.Select(MapearHorario);
    }

    public async Task<IEnumerable<HorarioDoctorResponseDto>> ObtenerPorDoctorAsync(Guid doctorId)
    {
        var horarios = await _horarioRepository.ObtenerPorDoctorAsync(doctorId);

        return horarios.Select(MapearHorario);
    }

    public async Task<Guid> CrearAsync(CrearHorarioDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor == null)
            throw new KeyNotFoundException("Doctor no encontrado.");

        if (dto.HoraFin <= dto.HoraInicio)
            throw new InvalidOperationException("La hora de fin debe ser mayor que la hora de inicio.");
        
        if (dto.FechaFinVigencia.HasValue && dto.FechaFinVigencia.Value < dto.FechaInicioVigencia)
            throw new InvalidOperationException("La fecha de fin de vigencia no puede ser menor que la fecha de inicio.");

        var horario = new HorarioDoctor
        {
            Id = Guid.NewGuid(),
            DoctorId = dto.DoctorId,
            DiaSemana = dto.DiaSemana,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            FechaInicioVigencia = dto.FechaInicioVigencia,
            FechaFinVigencia = dto.FechaFinVigencia,
            Activo = true
        };

        await _horarioRepository.AddAsync(horario);
        await _horarioRepository.SaveChangesAsync();

        return horario.Id;
    }

    public async Task ActualizarAsync(Guid id, EditarHorarioDoctorDto dto)
    {
        var horario = await _horarioRepository.GetByIdAsync(id);
        if (horario == null)
            throw new KeyNotFoundException("Horario no encontrado.");

        if (dto.HoraFin <= dto.HoraInicio)
            throw new InvalidOperationException("La hora de fin debe ser mayor que la hora de inicio.");
        
        if (dto.FechaFinVigencia.HasValue && dto.FechaFinVigencia.Value < dto.FechaInicioVigencia)
            throw new InvalidOperationException("La fecha de fin de vigencia no puede ser menor que la fecha de inicio.");

        horario.DiaSemana = dto.DiaSemana;
        horario.HoraInicio = dto.HoraInicio;
        horario.HoraFin = dto.HoraFin;
        horario.FechaInicioVigencia = dto.FechaInicioVigencia;
        horario.FechaFinVigencia = dto.FechaFinVigencia;
        horario.Activo = dto.Activo;

        _horarioRepository.Update(horario);
        await _horarioRepository.SaveChangesAsync();
    }

    private static HorarioDoctorResponseDto MapearHorario(HorarioDoctor horario)
    {
        return new HorarioDoctorResponseDto
        {
            Id = horario.Id,
            DoctorId = horario.DoctorId,
            DoctorNombre = horario.Doctor == null ? string.Empty : $"{horario.Doctor.Nombres} {horario.Doctor.Apellidos}",
            DiaSemana = horario.DiaSemana,
            HoraInicio = horario.HoraInicio,
            HoraFin = horario.HoraFin,
            FechaInicioVigencia = horario.FechaInicioVigencia,
            FechaFinVigencia = horario.FechaFinVigencia,
            Activo = horario.Activo
        };
    }
}