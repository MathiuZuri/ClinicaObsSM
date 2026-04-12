using Clinica.Domain.DTOs;

namespace Clinica.API.Services;

public interface IPacienteService
{
    Task<IReadOnlyList<PacienteResponseDto>> ObtenerTodosAsync();
    Task<PacienteResponseDto?> ObtenerPorIdAsync(Guid id);
    Task<PacienteResponseDto?> ObtenerPorDniAsync(string dni);
    Task<Guid> CrearAsync(CrearPacienteDto dto);
    Task ActualizarContactoAsync(Guid id, ActualizarContactoPacienteDto dto);

}