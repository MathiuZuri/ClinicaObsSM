using System.Text;
using Clinica.API.Helpers;
using Clinica.API.Services;
using Clinica.API.Services.Imp;
using Clinica.Domain.Interfaces;
using Clinica.Infrastructure.Data;
using Clinica.Infrastructure.Data.Seeds;
using Clinica.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Clinica.API.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<JwtHelper>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    foreach (var permiso in PermisosPolicies.Todos)
    {
        options.AddPolicy(permiso, policy =>
            policy.RequireClaim("permiso", permiso));
    }
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorio genérico
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IHorarioDoctorRepository, HorarioDoctorRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();

builder.Services.AddScoped<IServicioClinicoRepository, ServicioClinicoRepository>();
builder.Services.AddScoped<IHistorialClinicoRepository, HistorialClinicoRepository>();
builder.Services.AddScoped<IHistorialDetalleRepository, HistorialDetalleRepository>();
builder.Services.AddScoped<IAtencionRepository, AtencionRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IHorarioDoctorService, HorarioDoctorService>();
builder.Services.AddScoped<ICitaService, CitaService>();

builder.Services.AddScoped<IServicioClinicoService, ServicioClinicoService>();
builder.Services.AddScoped<IHistorialClinicoService, HistorialClinicoService>();
builder.Services.AddScoped<IAtencionService, AtencionService>();
builder.Services.AddScoped<IPagoService, PagoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7299")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("PermitirBlazor");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DataSeeder.SeedAsync(context);
}

app.Run(); // NOSONAR