using MudBlazor;

namespace BDAplication.Web.Helpers;

public static class AppTheme
{
    public static readonly MudTheme Current = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary          = "#1565c0",
            PrimaryDarken    = "#0d47a1",
            PrimaryLighten   = "#1976d2",
            Secondary        = "#546e7a",
            SecondaryDarken  = "#37474f",
            Background       = "#f4f5f7",   // off-white — separa superficie de fondo
            BackgroundGray   = "#eef0f5",
            Surface          = "#ffffff",
            AppbarBackground = "#1565c0",
            AppbarText       = "rgba(255,255,255,0.95)",
            DrawerBackground = "#1a1f36",
            DrawerText       = "rgba(255,255,255,0.87)",
            DrawerIcon       = "rgba(255,255,255,0.7)",
            TextPrimary      = "#1c2333",
            TextSecondary    = "#546e7a",
            Divider          = "#e0e3ea",
            DividerLight     = "#eef0f5",
            Success          = "#2e7d32",
            Warning          = "#e65100",
            Error            = "#c62828",
            Info             = "#0277bd",
            GrayDefault      = "#90a4ae",
            GrayLight        = "#cfd8dc",
            GrayLighter      = "#eceff1",
            OverlayLight     = "rgba(255,255,255,0.5)",
        },
        PaletteDark = new PaletteDark
        {
            Primary          = "#1976d2",
            PrimaryDarken    = "#0d47a1",
            PrimaryLighten   = "#42a5f5",
            Secondary        = "#78909c",
            Background       = "#111827",
            BackgroundGray   = "#1a2236",
            Surface          = "#1e2535",
            AppbarBackground = "#0d1b30",
            AppbarText       = "rgba(255,255,255,0.95)",
            DrawerBackground = "#0f1623",
            DrawerText       = "rgba(255,255,255,0.87)",
            DrawerIcon       = "rgba(255,255,255,0.6)",
            TextPrimary      = "#e8edf5",
            TextSecondary    = "#8a9ab5",
            Divider          = "#2d3a50",
            DividerLight     = "#1e2b40",
            Success          = "#43a047",
            Warning          = "#fb8c00",
            Error            = "#ef5350",
            Info             = "#0288d1",
            OverlayLight     = "rgba(30,37,53,0.6)",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px",
            DrawerWidthLeft     = "240px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif" },
                FontSize   = ".875rem",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "normal",
            },
            H4 = new H4Typography { FontSize = "1.75rem", FontWeight = "700", LetterSpacing = "-.02em" },
            H5 = new H5Typography { FontSize = "1.25rem",  FontWeight = "700", LetterSpacing = "-.01em" },
            H6 = new H6Typography { FontSize = "1rem",     FontWeight = "700", LetterSpacing = "normal" },
            Subtitle1 = new Subtitle1Typography { FontSize = ".875rem", FontWeight = "700" },
            Subtitle2 = new Subtitle2Typography { FontSize = ".8rem",   FontWeight = "600" },
            Body1     = new Body1Typography     { FontSize = ".875rem", FontWeight = "400", LineHeight = "1.5" },
            Body2     = new Body2Typography     { FontSize = ".8rem",   FontWeight = "400", LineHeight = "1.43" },
            Button    = new ButtonTypography    { FontSize = ".8125rem", FontWeight = "600", TextTransform = "none", LetterSpacing = ".02em" },
            Caption   = new CaptionTypography   { FontSize = ".75rem",  FontWeight = "400", LetterSpacing = ".02em" },
            Overline  = new OverlineTypography  { FontSize = ".6875rem", FontWeight = "700", LetterSpacing = ".06em" },
        }
    };
}
