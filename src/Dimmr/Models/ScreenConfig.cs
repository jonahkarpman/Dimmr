namespace Dimmr.Models;

/// <summary>
/// Per-monitor dim configuration inside a profile. Bounds are stored in physical
/// pixels so overlays can be placed exactly, which is what fixes coverage on docked
/// or mixed-DPI displays.
/// </summary>
public sealed class ScreenConfig
{
    /// <summary>Windows device name, for example \\.\DISPLAY1. Used to match a monitor.</summary>
    public string DeviceName { get; set; } = "";

    /// <summary>Friendly label shown in the UI.</summary>
    public string Label { get; set; } = "";

    /// <summary>Whether this screen participates in dimming.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>This screen's dim level as a percentage, 0 to <see cref="AppConstants.MaxDim"/>.</summary>
    public int Dim { get; set; } = 30;

    /// <summary>
    /// When true, the overlay uses the monitor bounds detected at runtime. When false,
    /// the manual X/Y/Width/Height below are used (the override for misdetected docks).
    /// </summary>
    public bool AutoBounds { get; set; } = true;

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
