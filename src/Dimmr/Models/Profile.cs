using System;
using System.Collections.Generic;

namespace Dimmr.Models;

/// <summary>
/// A named set of screens with their dim levels. One profile per physical setup,
/// for example "desk-g9" or "dual-external".
/// </summary>
public sealed class Profile
{
    public string Name { get; set; } = "DEFAULT";

    /// <summary>Master switch. When on, all enabled screens are dimmed.</summary>
    public bool MasterOn { get; set; }

    /// <summary>Master dim percentage, 0 to <see cref="AppConstants.MaxDim"/>.</summary>
    public int MasterDim { get; set; } = 30;

    public List<ScreenConfig> Screens { get; set; } = new();

    /// <summary>The dim percentage actually applied to a screen right now.</summary>
    public int EffectiveDim(ScreenConfig screen)
        => (MasterOn && screen.Enabled)
            ? Math.Clamp(screen.Dim, 0, AppConstants.MaxDim)
            : 0;
}
