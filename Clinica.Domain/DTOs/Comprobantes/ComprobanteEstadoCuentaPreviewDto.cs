namespace Clinica.Domain.DTOs.Comprobantes;

public class ComprobanteEstadoCuentaPreviewDto
{
    public Guid PacienteId { get; set; }

    public string Paciente { get; set; } = string.Empty;
    public string DniPaciente { get; set; } = string.Empty;

    public decimal TotalFacturado { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal TotalPendiente { get; set; }

    public int CantidadPagos { get; set; }

    public List<ComprobantePagoPreviewDto> Pagos { get; set; } = new();
}