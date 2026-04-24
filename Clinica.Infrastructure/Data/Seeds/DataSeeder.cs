using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinica.Infrastructure.Data.Seeds;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        await SeedPermisosAsync(context);
        await SeedRolesAsync(context);
        await SeedUsuarioAdminAsync(context);
        await SeedServiciosClinicosAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedPermisosAsync(ApplicationDbContext context)
    {
        if (await context.Permisos.AnyAsync()) return;

        var permisos = new List<Permiso>
        {
            new() { Codigo = "PACIENTE_VER", Nombre = "Ver pacientes", Modulo = "Pacientes", Activo = true },
            new() { Codigo = "PACIENTE_CREAR", Nombre = "Crear pacientes", Modulo = "Pacientes", Activo = true },
            new() { Codigo = "PACIENTE_EDITAR", Nombre = "Editar pacientes", Modulo = "Pacientes", Activo = true },

            new() { Codigo = "CITA_PROGRAMAR", Nombre = "Programar citas", Modulo = "Citas", Activo = true },
            new() { Codigo = "CITA_CANCELAR", Nombre = "Cancelar citas", Modulo = "Citas", Activo = true },

            new() { Codigo = "ATENCION_REGISTRAR", Nombre = "Registrar atención", Modulo = "Atenciones", Activo = true },
            new() { Codigo = "PAGO_REGISTRAR", Nombre = "Registrar pago", Modulo = "Pagos", Activo = true }
        };

        await context.Permisos.AddRangeAsync(permisos);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        if (await context.Roles.AnyAsync()) return;

        var adminRol = new Rol
        {
            Nombre = "Administrador",
            Descripcion = "Rol principal del sistema con acceso total.",
            EsSistema = true,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var recepcionistaRol = new Rol
        {
            Nombre = "Recepcionista",
            Descripcion = "Gestiona pacientes y citas.",
            EsSistema = true,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var doctorRol = new Rol
        {
            Nombre = "Doctor",
            Descripcion = "Gestiona atenciones médicas.",
            EsSistema = true,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        var cajaRol = new Rol
        {
            Nombre = "Caja",
            Descripcion = "Gestiona pagos y cobros.",
            EsSistema = true,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        await context.Roles.AddRangeAsync(adminRol, recepcionistaRol, doctorRol, cajaRol);
        await context.SaveChangesAsync();

        var permisos = await context.Permisos.ToListAsync();

        foreach (var permiso in permisos)
        {
            context.RolPermisos.Add(new RolPermiso
            {
                RolId = adminRol.Id,
                PermisoId = permiso.Id,
                FechaAsignacion = DateTime.UtcNow
            });
        }
    }

    private static async Task SeedUsuarioAdminAsync(ApplicationDbContext context)
    {
        if (await context.Usuarios.AnyAsync()) return;

        var admin = new Usuario
        {
            CodigoUsuario = $"USR-{DateTime.UtcNow:yyyy}-ADMIN",
            Nombres = "Administrador",
            Apellidos = "Sistema",
            UserName = "admin",
            Correo = "admin@clinica.com",
            PasswordHash = "admin123",
            Estado = EstadoUsuario.Activo,
            FechaRegistro = DateTime.UtcNow
        };

        await context.Usuarios.AddAsync(admin);
        await context.SaveChangesAsync();

        var rolAdmin = await context.Roles.FirstAsync(x => x.Nombre == "Administrador");

        context.UsuarioRoles.Add(new UsuarioRol
        {
            UsuarioId = admin.Id,
            RolId = rolAdmin.Id,
            FechaAsignacion = DateTime.UtcNow,
            Activo = true
        });
    }

    private static async Task SeedServiciosClinicosAsync(ApplicationDbContext context)
    {
        if (await context.ServiciosClinicos.AnyAsync()) return;

        var servicios = new List<ServicioClinico>
        {
            new()
            {
                CodigoServicio = "ATEGEN",
                Nombre = "Atención genérica",
                Descripcion = "Servicio clínico base para registrar una atención general.",
                CostoBase = 50,
                DuracionMinutos = 30,
                RequiereCita = true,
                GeneraHistorial = true,
                Estado = EstadoServicioClinico.Activo
            },

            new()
            {
                CodigoServicio = "CONOBS",
                Nombre = "Consulta obstétrica",
                Descripcion = "Consulta médica orientada al control obstétrico.",
                CostoBase = 70,
                DuracionMinutos = 30,
                RequiereCita = true,
                GeneraHistorial = true,
                Estado = EstadoServicioClinico.Activo
            }
        };

        await context.ServiciosClinicos.AddRangeAsync(servicios);
    }
}