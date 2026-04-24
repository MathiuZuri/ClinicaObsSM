namespace Clinica.Domain.DTOs.Citas;

public class CancelarCitaDto
{
    public string MotivoCancelacion { get; set; } = string.Empty;
    public Guid? UsuarioRegistroId { get; set; }
}