namespace Clinica.Domain.Enums;

public enum TipoAccionAuditoria
{
    InicioSesion = 1,
    CierreSesion = 2,
    InicioSesionFallido = 3,

    Crear = 10,
    Editar = 11,
    Eliminar = 12,
    EliminarLogico = 13,
    CambiarEstado = 14,
    Consultar = 15,

    ProgramarCita = 20,
    ReprogramarCita = 21,
    CancelarCita = 22,
    RegistrarAtencion = 23,
    CerrarAtencion = 24,

    RegistrarPago = 30,
    AnularPago = 31,
    ReembolsarPago = 32,

    AsignarRol = 40,
    QuitarRol = 41,
    AsignarPermiso = 42,
    QuitarPermiso = 43,

    ExportarReporte = 50,
    ErrorSistema = 99
}