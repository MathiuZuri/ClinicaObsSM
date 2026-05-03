namespace Clinica.API.Services;

// esto es exclusivo de evolution api, no incluir al sistema
public interface INotificacionWhatsAppService
{
    Task EnviarMensajeAsync(string telefonoDestino, string mensaje, CancellationToken cancellationToken = default);
}