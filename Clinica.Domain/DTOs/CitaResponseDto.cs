using Clinica.Domain.Enums;

namespace Clinica.Domain.DTOs;

public record CitaResponseDto(
    Guid Id,
    Guid PacienteId,
    DateTime? FechaHoraProgramada,
    string Servicio,
    EstadoCita Estado
);