namespace Clinica.Domain.DTOs;

public record PacienteResponseDto(
    Guid Id, 
    string DNI, 
    string NumeroHC, 
    string Nombres, 
    string Apellidos, 
    DateTime FechaNacimiento,
    string? Celular);