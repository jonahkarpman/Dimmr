using System;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Dimmr.Services;

/// <summary>
/// Swaps the app's color scheme at runtime by recoloring the shared brush resources in
/// place, so every control that references them updates live. Two schemes: phosphor green
/// (default) and New Vegas amber.
/// </summary>
public static class Palette
{
    /// <summary>The current glow color, used by the title and screen-box glow effects.</summary>
    public static Color GlowColor { get; private set; } = ToColor("#00FF41");

    public static void Apply(string scheme)
    {
        if (string.Equals(scheme, "Amber", StringComparison.OrdinalIgnoreCase))
            ApplyAmber();
        else
            ApplyGreen();
    }

    private static void ApplyGreen()
    {
        Set("BackgroundBrush", "#0A0A0A");
        Set("SurfaceBrush", "#0D1A0D");
        Set("PrimaryTextBrush", "#33FF33");
        Set("SecondaryTextBrush", "#1A8C1A");
        Set("AccentBrush", "#00FF41");
        Set("DangerBrush", "#FF8800");
        Set("BorderBrush", "#1A4D1A");
        Set("DisabledBrush", "#0D3D0D");
        Set("HoverBrush", "#3333FF33");
        GlowColor = ToColor("#00FF41");
    }

    private static void ApplyAmber()
    {
        Set("BackgroundBrush", "#0A0A0A");
        Set("SurfaceBrush", "#1A140A");
        Set("PrimaryTextBrush", "#FFB642");
        Set("SecondaryTextBrush", "#A67C33");
        Set("AccentBrush", "#FFCC66");
        Set("DangerBrush", "#FFE0A0");
        Set("BorderBrush", "#4D3A1A");
        Set("DisabledBrush", "#3D2E0D");
        Set("HoverBrush", "#33FFB642");
        GlowColor = ToColor("#FFB642");
    }

    private static void Set(string key, string hex)
    {
        if (Application.Current?.Resources[key] is SolidColorBrush brush && !brush.IsFrozen)
            brush.Color = ToColor(hex);
    }

    private static Color ToColor(string hex) => (Color)ColorConverter.ConvertFromString(hex);
}
