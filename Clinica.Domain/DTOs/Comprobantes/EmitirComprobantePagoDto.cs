using Clinica.Domain.Enums;

namespace Clinica.Domain.DTOs.Comprobantes;

public class EmitirComprobantePagoDto
{
    public Guid PagoId { get; set; }

    public decimal TasaImpuesto { get; set; } = 0.18m;

    public TipoFormatoImpresion FormatoImpresion { get; set; } = TipoFormatoImpresion.A4;

    public string? Observacion { get; set; }
}