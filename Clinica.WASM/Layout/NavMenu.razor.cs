using Clinica.WASM.Constants;
using Clinica.WASM.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace Clinica.WASM.Layout;

public partial class NavMenu : ComponentBase
{
    [Inject] private AuthStateService AuthStateService { get; set; } = default!;

    protected bool PuedeVerPacientes { get; set; }
    protected bool PuedeVerCitas { get; set; }
    protected bool PuedeVerAtenciones { get; set; }
    protected bool PuedeVerDoctores { get; set; }
    protected bool PuedeVerHorarios { get; set; }
    protected bool PuedeVerFinanzas { get; set; }
    protected bool PuedeVerComprobantes { get; set; }
    protected bool PuedeVerHistorial { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PuedeVerPacientes = await AuthStateService.TienePermisoAsync(Permisos.PacienteVer);
        PuedeVerCitas = await AuthStateService.TienePermisoAsync(Permisos.CitaVer);
        PuedeVerAtenciones = await AuthStateService.TienePermisoAsync(Permisos.AtencionVer);
        PuedeVerDoctores = await AuthStateService.TienePermisoAsync(Permisos.DoctorVer);
        PuedeVerHorarios = await AuthStateService.TienePermisoAsync(Permisos.HorarioVer);
        PuedeVerFinanzas = await AuthStateService.TienePermisoAsync(Permisos.FinanzasVer);
        PuedeVerComprobantes = await AuthStateService.TienePermisoAsync(Permisos.ComprobanteVer);
        PuedeVerHistorial = await AuthStateService.TienePermisoAsync(Permisos.HistorialVer);
    }
}