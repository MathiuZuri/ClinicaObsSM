using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Estas propiedades representan las tablas en PostgreSQL
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<PersonalMedico> PersonalMedico => Set<PersonalMedico>();
    public DbSet<Cita> Citas => Set<Cita>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la tabla Usuarios
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.Id);
            
            // Garantiza que no existan dos correos iguales a nivel de base de datos
            entity.HasIndex(e => e.Correo).IsUnique(); 
        });

        // Configuración de la tabla Pacientes
        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("Pacientes");
            entity.HasKey(e => e.Id);
            
            // Índices únicos para evitar duplicados críticos
            entity.HasIndex(e => e.DNI).IsUnique();
            entity.HasIndex(e => e.NumeroHC).IsUnique();
            
            // Relación: Un Paciente está vinculado a un Usuario
            // Usamos Restrict para evitar que borrar un usuario borre al paciente por accidente
            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de la tabla PersonalMedico
        modelBuilder.Entity<PersonalMedico>(entity =>
        {
            entity.ToTable("PersonalMedico");
            entity.HasKey(e => e.Id);
            
            entity.HasOne<Usuario>()
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de la tabla Citas
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.ToTable("Citas");
            entity.HasKey(e => e.Id);

            // Relación con el paciente
            entity.HasOne<Paciente>()
                  .WithMany()
                  .HasForeignKey(e => e.PacienteId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación con el médico (es opcional, por lo que acepta nulos si no se ha asignado)
            entity.HasOne<PersonalMedico>()
                  .WithMany()
                  .HasForeignKey(e => e.PersonalMedicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}