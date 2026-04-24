using Clinica.Domain.DTOs.Doctores;
using Clinica.Domain.Entities;
using Clinica.Domain.Interfaces;

namespace Clinica.API.Services.Imp;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<DoctorResponseDto>> ObtenerTodosAsync()
    {
        var doctores = await _doctorRepository.GetAllAsync();

        return doctores.Select(MapearDoctor);
    }

    public async Task<IEnumerable<DoctorResponseDto>> ObtenerActivosAsync()
    {
        var doctores = await _doctorRepository.ObtenerActivosAsync();

        return doctores.Select(MapearDoctor);
    }

    public async Task<DoctorResponseDto?> ObtenerPorIdAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) return null;

        return MapearDoctor(doctor);
    }

    public async Task<Guid> CrearAsync(CrearDoctorDto dto)
    {
        var existe = await _doctorRepository.ObtenerPorCmpAsync(dto.CMP);
        if (existe != null)
            throw new InvalidOperationException("Ya existe un doctor registrado con ese CMP.");

        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            CodigoDoctor = GenerarCodigoDoctor(dto.CMP),
            CMP = dto.CMP,
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Especialidad = dto.Especialidad,
            Celular = dto.Celular,
            Correo = dto.Correo,
            FechaInicioContrato = dto.FechaInicioContrato,
            FechaFinContrato = dto.FechaFinContrato,
            UsuarioId = dto.UsuarioId
        };

        await _doctorRepository.AddAsync(doctor);
        await _doctorRepository.SaveChangesAsync();

        return doctor.Id;
    }

    public async Task ActualizarAsync(Guid id, EditarDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null)
            throw new KeyNotFoundException("Doctor no encontrado.");

        doctor.CMP = dto.CMP;
        doctor.Nombres = dto.Nombres;
        doctor.Apellidos = dto.Apellidos;
        doctor.Especialidad = dto.Especialidad;
        doctor.Celular = dto.Celular;
        doctor.Correo = dto.Correo;
        doctor.FechaInicioContrato = dto.FechaInicioContrato;
        doctor.FechaFinContrato = dto.FechaFinContrato;
        doctor.Estado = dto.Estado;

        _doctorRepository.Update(doctor);
        await _doctorRepository.SaveChangesAsync();
    }

    private static DoctorResponseDto MapearDoctor(Doctor doctor)
    {
        return new DoctorResponseDto
        {
            Id = doctor.Id,
            CodigoDoctor = doctor.CodigoDoctor,
            CMP = doctor.CMP,
            Nombres = doctor.Nombres,
            Apellidos = doctor.Apellidos,
            Especialidad = doctor.Especialidad,
            Celular = doctor.Celular,
            Correo = doctor.Correo,
            FechaInicioContrato = doctor.FechaInicioContrato,
            FechaFinContrato = doctor.FechaFinContrato,
            Estado = doctor.Estado
        };
    }

    private static string GenerarCodigoDoctor(string cmp)
    {
        return $"DOC-{Guid.NewGuid().ToString("N")[..5].ToUpper()}-{cmp}";
    }
}