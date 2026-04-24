namespace Clinica.Domain.DTOs.Atenciones;

public class CerrarAtencionDto
{
    public string? DiagnosticoResumen { get; set; }
    public string? Indicaciones { get; set; }
    public string? Tratamiento { get; set; }
    public string? ObservacionesFinales { get; set; }

    public Guid? UsuarioRegistroId { get; set; }
}