namespace Clinica.Domain.DTOs.Pacientes;

public class CrearPacienteDto
{
    public string DNI { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    public string? Celular { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }

    public Guid UsuarioId { get; set; }
}