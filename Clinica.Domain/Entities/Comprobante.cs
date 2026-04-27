namespace Clinica.Domain.Entities;

public class Comprobante
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CodigoComprobante { get; set; } = string.Empty;

    public string Serie { get; set; } = string.Empty;
    public int Numero { get; set; }

    public TipoComprobante TipoComprobante { get; set; }
    public EstadoComprobante Estado { get; set; } = EstadoComprobante.Emitido;

    public Guid PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public Guid? PagoId { get; set; }
    public Pago? Pago { get; set; }

    public Guid? CitaId { get; set; }
    public Cita? Cita { get; set; }

    public Guid? AtencionId { get; set; }
    public Atencion? Atencion { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TasaImpuesto { get; set; }
    public decimal MontoImpuesto { get; set; }
    public decimal Total { get; set; }

    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;

    public Guid UsuarioEmisionId { get; set; }
    public Usuario UsuarioEmision { get; set; } = null!;

    public string? Observacion { get; set; }

    public string DatosSnapshotJson { get; set; } = string.Empty;

    public ICollection<ComprobanteDetalle> Detalles { get; set; } = new List<ComprobanteDetalle>();
}