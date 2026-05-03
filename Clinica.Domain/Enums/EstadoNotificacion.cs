namespace Clinica.Domain.Enums;

public enum EstadoNotificacion
{
    // esto es exclusivo de evolution api, no incluir al sistema
    Pendiente = 1,
    Enviado = 2,
    Fallido = 3,
    Cancelado = 4
}