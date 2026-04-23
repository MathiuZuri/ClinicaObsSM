namespace Clinica.Domain.DTOs;

public class CrearPacienteDto
{
    // Nota: Por ahora pedimos el UsuarioId, pero en el futuro esto 
    // se sacará automáticamente del token de quien inició sesión.
    public Guid UsuarioId { get; set; } 
    public string DNI { get; set; } = string.Empty;
    public string NumeroHC { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "F"; // Por defecto Femenino en obstetricia
    
    public string? Celular { get; set; }
    public string? CorreoSecundario { get; set; }
    public string? Direccion { get; set; }
}