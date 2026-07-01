namespace Dimmr.Models;

/// <summary>Global, profile-independent settings.</summary>
public sealed class AppSettings
{
    public string ActiveProfile { get; set; } = "DEFAULT";

    /// <summary>Launch with Windows (mirrored to the HKCU Run key).</summary>
    public bool RunAtStartup { get; set; }

    /// <summary>
    /// When true, dim is forced to <see cref="StartupDim"/> on launch. When false, the
    /// active profile's last saved master dim is restored instead.
    /// </summary>
    public bool ResetDimOnStartup { get; set; } = true;

    /// <summary>Dim percentage applied on launch when <see cref="ResetDimOnStartup"/> is true.</summary>
    public int StartupDim { get; set; }

    public bool DimmingOn { get; set; } = true;
    public bool SoundsEnabled { get; set; }
    public bool Hum { get; set; }
    public bool Scanlines { get; set; } = true;
    public bool Glow { get; set; }
}
