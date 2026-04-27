using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinica.Infrastructure.Data.Configurations;

public class ComprobanteDetalleConfiguration : IEntityTypeConfiguration<ComprobanteDetalle>
{
    public void Configure(EntityTypeBuilder<ComprobanteDetalle> builder)
    {
        builder.ToTable("ComprobanteDetalles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Descripcion)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.Cantidad)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.PrecioUnitario)
            .HasPrecision(10, 2)
            .IsRequired();

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

        builder.HasIndex(x => x.ComprobanteId);

        builder.HasOne(x => x.Comprobante)
            .WithMany(x => x.Detalles)
            .HasForeignKey(x => x.ComprobanteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}