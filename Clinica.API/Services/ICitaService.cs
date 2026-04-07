using Clinica.Domain.DTOs;

namespace Clinica.API.Services;

public interface ICitaService
{
    Task<CitaResponseDto> AgendarNuevaCitaAsync(CrearCitaDto dto);
    Task<IEnumerable<CitaResponseDto>> ObtenerTodasAsync();
    Task<CitaResponseDto?> ObtenerPorIdAsync(Guid id);
    Task ActualizarAsync(Guid id, CrearCitaDto dto);
    Task EliminarAsync(Guid id);
}