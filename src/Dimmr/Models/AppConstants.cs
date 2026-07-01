namespace Dimmr.Models;

public static class AppConstants
{
    /// <summary>Maximum dim percentage. Capped below 100 so a screen never goes fully black.</summary>
    public const int MaxDim = 85;

    /// <summary>Step per brightness hotkey press, in percent.</summary>
    public const int DimStep = 5;

    public const string AppName = "Dimmr";
    public const string Version = "0.3.0";
}
