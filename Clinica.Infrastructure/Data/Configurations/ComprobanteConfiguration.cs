using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinica.Infrastructure.Data.Configurations;

public class ComprobanteConfiguration : IEntityTypeConfiguration<Comprobante>
{
    public void Configure(EntityTypeBuilder<Comprobante> builder)
    {
        builder.ToTable("Comprobantes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Serie)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(x => x.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.CodigoComprobante)
            .IsRequired()
            .HasMaxLength(60);

        builder.HasIndex(x => x.CodigoComprobante)
            .IsUnique();

        builder.HasIndex(x => new { x.Serie, x.Numero })
            .IsUnique();

        builder.Property(x => x.TipoComprobante)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.Estado)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.TipoDocumentoPaciente)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.FormatoImpresion)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.NumeroDocumentoPaciente)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.NombrePaciente)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DireccionPaciente)
            .HasMaxLength(250);

        builder.Property(x => x.Subtotal)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.PorcentajeImpuesto)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.MontoImpuesto)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Total)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Observacion)
            .HasMaxLength(700);

        builder.Property(x => x.MotivoAnulacion)
            .HasMaxLength(700);

        builder.Property(x => x.SnapshotJson)
            .HasColumnType("jsonb");

        builder.Property(x => x.FechaEmision)
            .IsRequired();

        builder.Property(x => x.FechaAnulacion);

        builder.HasIndex(x => x.PacienteId);
        builder.HasIndex(x => x.PagoId);
        builder.HasIndex(x => x.CitaId);
        builder.HasIndex(x => x.AtencionId);
        builder.HasIndex(x => x.FechaEmision);
        builder.HasIndex(x => x.Estado);

        builder.HasOne(x => x.Paciente)
            .WithMany()
            .HasForeignKey(x => x.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Pago)
            .WithMany()
            .HasForeignKey(x => x.PagoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Cita)
            .WithMany()
            .HasForeignKey(x => x.CitaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Atencion)
            .WithMany()
            .HasForeignKey(x => x.AtencionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UsuarioEmision)
            .WithMany()
            .HasForeignKey(x => x.UsuarioEmisionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UsuarioAnulacion)
            .WithMany()
            .HasForeignKey(x => x.UsuarioAnulacionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Detalles)
            .WithOne(x => x.Comprobante)
            .HasForeignKey(x => x.ComprobanteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}