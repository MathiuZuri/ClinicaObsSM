using System.Security.Claims;
using System.Text.Json;
using Clinica.Domain.Entities;
using Clinica.Domain.Enums;
using Clinica.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Clinica.API.Filters;

public class AuditoriaAutomaticaFilter : IAsyncActionFilter
{
    private readonly ApplicationDbContext _context;

    public AuditoriaAutomaticaFilter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ejecutado = await next();

        var http = context.HttpContext;
        var endpoint = context.ActionDescriptor as ControllerActionDescriptor;

        var atributo = endpoint?.MethodInfo
            .GetCustomAttributes(typeof(AuditoriaAttribute), true)
            .OfType<AuditoriaAttribute>()
            .FirstOrDefault();

        var usuarioIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        Guid? usuarioId = Guid.TryParse(usuarioIdClaim, out var idUsuario)
            ? idUsuario
            : null;

        var statusCode = http.Response.StatusCode;

        var fueExitoso = statusCode >= 200 && statusCode < 400 && ejecutado.Exception == null;

        var auditoria = new Auditoria
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            TipoAccion = atributo?.TipoAccion ?? DetectarTipoAccion(http.Request.Method),
            Nivel = atributo?.Nivel ?? DetectarNivel(http.Request.Method),
            Modulo = atributo?.Modulo ?? endpoint?.ControllerName ?? "Sistema",
            EntidadAfectada = atributo?.Entidad ?? endpoint?.ControllerName ?? "General",
            EntidadId = ObtenerEntidadId(context),
            Descripcion = GenerarDescripcion(http.Request.Method, endpoint, fueExitoso, statusCode),
            ValorAnterior = null,
            ValorNuevo = JsonSerializer.Serialize(new
            {
                Metodo = http.Request.Method,
                Ruta = http.Request.Path.Value,
                Query = http.Request.QueryString.Value,
                StatusCode = statusCode,
                Accion = endpoint?.ActionName,
                Controlador = endpoint?.ControllerName
            }),
            IpAddress = http.Connection.RemoteIpAddress?.ToString(),
            UserAgent = http.Request.Headers.UserAgent.ToString(),
            FueExitoso = fueExitoso,
            DetalleError = ejecutado.Exception?.Message,
            FechaHora = DateTime.UtcNow
        };

        _context.Auditorias.Add(auditoria);
        await _context.SaveChangesAsync();
    }

    private static TipoAccionAuditoria DetectarTipoAccion(string metodo)
    {
        return metodo.ToUpper() switch
        {
            "GET" => TipoAccionAuditoria.Consulta,
            "POST" => TipoAccionAuditoria.Creacion,
            "PUT" => TipoAccionAuditoria.Edicion,
            "PATCH" => TipoAccionAuditoria.Edicion,
            "DELETE" => TipoAccionAuditoria.Eliminacion,
            _ => TipoAccionAuditoria.Consulta
        };
    }

    private static NivelAuditoria DetectarNivel(string metodo)
    {
        return metodo.ToUpper() switch
        {
            "POST" => NivelAuditoria.Importante,
            "PUT" => NivelAuditoria.Importante,
            "PATCH" => NivelAuditoria.Importante,
            "DELETE" => NivelAuditoria.Critico,
            _ => NivelAuditoria.Normal
        };
    }

    private static Guid? ObtenerEntidadId(ActionExecutingContext context)
    {
        string? valor = null;

        if (context.RouteData.Values.TryGetValue("id", out var id))
            valor = id?.ToString();
        else if (context.RouteData.Values.TryGetValue("usuarioId", out var usuarioId))
            valor = usuarioId?.ToString();
        else if (context.RouteData.Values.TryGetValue("pacienteId", out var pacienteId))
            valor = pacienteId?.ToString();
        else if (context.RouteData.Values.TryGetValue("doctorId", out var doctorId))
            valor = doctorId?.ToString();
        else if (context.RouteData.Values.TryGetValue("citaId", out var citaId))
            valor = citaId?.ToString();
        else if (context.RouteData.Values.TryGetValue("atencionId", out var atencionId))
            valor = atencionId?.ToString();
        else if (context.RouteData.Values.TryGetValue("historialId", out var historialId))
            valor = historialId?.ToString();

        return Guid.TryParse(valor, out var guid) ? guid : null;
    }

    private static string GenerarDescripcion(
        string metodo,
        ControllerActionDescriptor? endpoint,
        bool fueExitoso,
        int statusCode)
    {
        var resultado = fueExitoso ? "exitosa" : "fallida";

        return $"Solicitud {metodo} ejecutada en {endpoint?.ControllerName}/{endpoint?.ActionName}. Resultado: {resultado}. Código HTTP: {statusCode}.";
    }
}