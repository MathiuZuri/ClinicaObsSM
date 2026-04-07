using Clinica.Domain.Enums;

namespace Clinica.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public bool Activo { get; set; }
    
    protected Usuario() { }

    public Usuario(string correo, string passwordHash, RolUsuario rol)
    {
        Id = Guid.NewGuid();
        Correo = correo.ToLower().Trim();
        PasswordHash = passwordHash;
        Rol = rol;
        Activo = true;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
    
    // Método para cuando el usuario pide recuperar contraseña
    public void ActualizarPassword(string nuevoHash)
    {
        PasswordHash = nuevoHash;
    }
}