using Clinica.Domain.Enums;

namespace Clinica.Domain.DTOs.Comprobantes;

public class EmitirComprobanteAtencionDto
{
    public Guid AtencionId { get; set; }

    public TipoFormatoImpresion FormatoImpresion { get; set; } = TipoFormatoImpresion.A4;

    public string? Observacion { get; set; }
}