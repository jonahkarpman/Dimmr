using System.Collections.Generic;
using System.Linq;
using Dimmr.Models;
using Dimmr.Views;

namespace Dimmr.Services;

/// <summary>Owns one overlay window per configured screen and keeps them in sync.</summary>
public sealed class OverlayManager
{
    private readonly List<(ScreenConfig Config, OverlayWindow Window)> _overlays = new();
    private Profile? _profile;

    /// <summary>Rebuilds all overlays for a profile (call on profile switch or display change).</summary>
    public void Apply(Profile profile)
    {
        _profile = profile;
        Rebuild();
    }

    public void Rebuild()
    {
        Clear();
        if (_profile is null)
            return;

        var monitors = MonitorService.GetMonitors();

        foreach (var screen in _profile.Screens)
        {
            int x, y, w, h;

            if (screen.AutoBounds)
            {
                var mon = monitors.FirstOrDefault(m => m.DeviceName == screen.DeviceName);
                if (mon is null)
                    continue; // monitor not connected in this setup
                (x, y, w, h) = (mon.X, mon.Y, mon.Width, mon.Height);
            }
            else
            {
                (x, y, w, h) = (screen.X, screen.Y, screen.Width, screen.Height);
                if (w <= 0 || h <= 0)
                    continue;
            }

            var window = new OverlayWindow();
            window.Show();
            window.Place(x, y, w, h);
            _overlays.Add((screen, window));
        }

        Refresh();
    }

    /// <summary>Updates opacity of existing overlays without recreating them.</summary>
    public void Refresh()
    {
        if (_profile is null)
            return;

        foreach (var (config, window) in _overlays)
            window.SetDim(_profile.EffectiveDim(config) / 100.0);
    }

    public void Clear()
    {
        foreach (var (_, window) in _overlays)
            window.Close();
        _overlays.Clear();
    }
}
