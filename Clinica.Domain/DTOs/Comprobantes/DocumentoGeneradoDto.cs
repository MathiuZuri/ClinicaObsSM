namespace Clinica.Domain.DTOs.Comprobantes;

public class DocumentoGeneradoDto
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = "application/pdf";
    public byte[] Contenido { get; set; } = Array.Empty<byte>();
}