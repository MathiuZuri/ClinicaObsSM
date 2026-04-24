using Clinica.Domain.Enums;

namespace Clinica.Domain.DTOs.Doctores;

public class EditarDoctorDto
{
    public string CMP { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public string? Correo { get; set; }

    public DateTime FechaInicioContrato { get; set; }
    public DateTime? FechaFinContrato { get; set; }

    public EstadoDoctor Estado { get; set; }
}