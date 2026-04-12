namespace Clinica.Domain.Entities;

public class Paciente
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; } 
    public string DNI { get; set; } = string.Empty;
    public string NumeroHC { get; set; } = string.Empty;
    
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Sexo { get; set; } = string.Empty;
    
    public string? Celular { get; set; }
    public string? CorreoSecundario { get; set; }
    public string? LugarNacimiento { get; set; }
    public string? Direccion { get; set; }
    
    public DateTime FechaCreacion { get; private set ; }

    protected Paciente() { }

    public Paciente(Guid usuarioId, string dni, string numeroHc, string nombres, string apellidos, DateTime fechaNacimiento, string sexo)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        DNI = dni;
        NumeroHC = numeroHc;
        Nombres = nombres.ToUpper().Trim();
        Apellidos = apellidos.ToUpper().Trim();
        FechaNacimiento = fechaNacimiento;
        Sexo = sexo.ToUpper();
        FechaCreacion = DateTime.UtcNow;
    }

    // Método para actualizar datos de contacto sin tocar datos sensibles como DNI o HC
    public void ActualizarContacto(string? celular, string? correoSecundario, string? direccion)
    {
        Celular = celular;
        CorreoSecundario = correoSecundario;
        Direccion = direccion;
    }
}