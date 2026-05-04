using MudBlazor;
using MudBlazor.Utilities;

namespace Clinica.WASM.Themes;

public class ClinicaTheme : MudTheme
{
    public ClinicaTheme()
    {
        PaletteLight = new PaletteLight
        {
            // Color institucional principal
            Primary = new MudColor("#3F65FF"),

            // Color secundario suave, relacionado a salud materna/obstétrica
            Secondary = new MudColor("#E4A4BB"),

            // Colores de apoyo
            Tertiary = new MudColor("#41B3A3"),
            Info = new MudColor("#2563EB"),
            Success = new MudColor("#16A34A"),
            Warning = new MudColor("#F59E0B"),
            Error = new MudColor("#DC2626"),

            // Fondos
            Background = new MudColor("#F6F8FC"),
            BackgroundGray = new MudColor("#EEF2F7"),
            Surface = new MudColor("#FFFFFF"),

            // Layout
            AppbarBackground = new MudColor("#FFFFFF"),
            AppbarText = new MudColor("#1E2430"),
            DrawerBackground = new MudColor("#FFFFFF"),
            DrawerText = new MudColor("#334155"),
            DrawerIcon = new MudColor("#64748B"),

            // Texto
            TextPrimary = new MudColor("#1E2430"),
            TextSecondary = new MudColor("#64748B"),

            // Líneas y divisores
            Divider = new MudColor("#E2E8F0"),
            LinesDefault = new MudColor("#E2E8F0"),
            TableLines = new MudColor("#E2E8F0"),

            // Tablas
            TableStriped = new MudColor("#F8FAFC"),
            TableHover = new MudColor("#EEF4FF"),

            // Acciones
            ActionDefault = new MudColor("#64748B"),
            ActionDisabled = new MudColor("#94A3B8"),
            ActionDisabledBackground = new MudColor("#E5E7EB")
        };

        PaletteDark = new PaletteDark
        {
            // Color institucional principal un poco más claro para fondo oscuro
            Primary = new MudColor("#6B8CFF"),

            // Rosado más suave para modo oscuro
            Secondary = new MudColor("#F0AFC7"),

            // Colores de apoyo
            Tertiary = new MudColor("#5EEAD4"),
            Info = new MudColor("#60A5FA"),
            Success = new MudColor("#22C55E"),
            Warning = new MudColor("#FBBF24"),
            Error = new MudColor("#F87171"),

            // Fondos oscuros, no negro puro
            Background = new MudColor("#0F172A"),
            BackgroundGray = new MudColor("#111827"),
            Surface = new MudColor("#1E293B"),

            // Layout
            AppbarBackground = new MudColor("#111827"),
            AppbarText = new MudColor("#F8FAFC"),
            DrawerBackground = new MudColor("#111827"),
            DrawerText = new MudColor("#E5E7EB"),
            DrawerIcon = new MudColor("#CBD5E1"),

            // Texto
            TextPrimary = new MudColor("#F8FAFC"),
            TextSecondary = new MudColor("#CBD5E1"),

            // Líneas y divisores
            Divider = new MudColor("#334155"),
            LinesDefault = new MudColor("#334155"),
            TableLines = new MudColor("#334155"),

            // Tablas
            TableStriped = new MudColor("#1E293B"),
            TableHover = new MudColor("#273449"),

            // Acciones
            ActionDefault = new MudColor("#CBD5E1"),
            ActionDisabled = new MudColor("#64748B"),
            ActionDisabledBackground = new MudColor("#1F2937")
        };

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Roboto", "system-ui", "sans-serif" },
                FontSize = "0.95rem",
                FontWeight = "400",
                LineHeight = "1.5"
            },

            H1 = new H1Typography
            {
                FontFamily = new[] { "Roboto", "system-ui", "sans-serif" },
                FontWeight = "700",
                FontSize = "2.2rem",
                LineHeight = "1.2"
            },

            H2 = new H2Typography
            {
                FontFamily = new[] { "Roboto", "system-ui", "sans-serif" },
                FontWeight = "700",
                FontSize = "1.8rem",
                LineHeight = "1.25"
            },

            H3 = new H3Typography
            {
                FontFamily = new[] { "Roboto", "system-ui", "sans-serif" },
                FontWeight = "600",
                FontSize = "1.45rem",
                LineHeight = "1.3"
            },

            Button = new ButtonTypography
            {
                FontFamily = new[] { "Roboto", "system-ui", "sans-serif" },
                FontWeight = "600",
                FontSize = "0.875rem",
                TextTransform = "none"
            }
        };

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "270px",
            DrawerWidthRight = "270px",
            AppbarHeight = "64px"
        };

        Shadows = new Shadow
        {
            Elevation = new[]
            {
                "none",
                "0 1px 2px rgba(15, 23, 42, 0.06)",
                "0 4px 12px rgba(15, 23, 42, 0.08)",
                "0 8px 24px rgba(15, 23, 42, 0.10)",
                "0 12px 32px rgba(15, 23, 42, 0.12)",
                "0 16px 40px rgba(15, 23, 42, 0.14)",
                "0 20px 48px rgba(15, 23, 42, 0.16)",
                "0 24px 56px rgba(15, 23, 42, 0.18)",
                "0 28px 64px rgba(15, 23, 42, 0.20)",
                "0 32px 72px rgba(15, 23, 42, 0.22)",
                "0 36px 80px rgba(15, 23, 42, 0.24)",
                "0 40px 88px rgba(15, 23, 42, 0.26)",
                "0 44px 96px rgba(15, 23, 42, 0.28)",
                "0 48px 104px rgba(15, 23, 42, 0.30)",
                "0 52px 112px rgba(15, 23, 42, 0.32)",
                "0 56px 120px rgba(15, 23, 42, 0.34)",
                "0 60px 128px rgba(15, 23, 42, 0.36)",
                "0 64px 136px rgba(15, 23, 42, 0.38)",
                "0 68px 144px rgba(15, 23, 42, 0.40)",
                "0 72px 152px rgba(15, 23, 42, 0.42)",
                "0 76px 160px rgba(15, 23, 42, 0.44)",
                "0 80px 168px rgba(15, 23, 42, 0.46)",
                "0 84px 176px rgba(15, 23, 42, 0.48)",
                "0 88px 184px rgba(15, 23, 42, 0.50)",
                "0 92px 192px rgba(15, 23, 42, 0.52)",
                "0 96px 200px rgba(15, 23, 42, 0.54)"
            }
        };
    }
}