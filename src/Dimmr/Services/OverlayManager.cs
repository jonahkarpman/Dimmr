using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Dimmr.Interop;
using Dimmr.Models;
using Dimmr.Views;

namespace Dimmr.Services;

/// <summary>
/// Owns one overlay window per monitor and keeps them in sync. Overlays are reused across
/// profile switches (keyed by device name), so changing profiles just updates opacity
/// instead of closing and recreating windows, which avoids a visible dim flash.
/// </summary>
public sealed class OverlayManager
{
    private readonly Dictionary<string, OverlayEntry> _overlays = new();
    private Profile? _profile;

    private sealed class OverlayEntry
    {
        public required OverlayWindow Window { get; init; }
        public required ScreenConfig Config { get; set; }
    }

    /// <summary>Reconciles overlays to the given profile, reusing existing windows.</summary>
    public void Apply(Profile profile)
    {
        _profile = profile;
        Reconcile();
    }

    /// <summary>Updates opacity of existing overlays without recreating them.</summary>
    /// <summary>Global on/off gate; when false, all overlays are clear.</summary>
    public bool DimmingOn { get; set; } = true;

    /// <summary>The app's own window, kept above the dim so it stays readable.</summary>
    public IntPtr ExcludedWindow { get; set; }

    public void Refresh()
    {
        if (_profile is null)
            return;
        foreach (var entry in _overlays.Values)
            entry.Window.SetDim(DimmingOn ? _profile.EffectiveDim(entry.Config) / 100.0 : 0.0);
        RaiseExcluded();
    }

    // Keep the app window above the overlays while dimming, so it is not dimmed itself.
    private void RaiseExcluded()
    {
        if (ExcludedWindow == IntPtr.Zero)
            return;
        var insertAfter = DimmingOn ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST;
        NativeMethods.SetWindowPos(
            ExcludedWindow, insertAfter, 0, 0, 0, 0,
            NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
    }

    private void Reconcile()
    {
        if (_profile is null)
        {
            Clear();
            return;
        }

        var monitors = MonitorService.GetMonitors();
        var wanted = new HashSet<string>();

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

            wanted.Add(screen.DeviceName);

            if (_overlays.TryGetValue(screen.DeviceName, out var entry))
            {
                entry.Config = screen;
                entry.Window.Place(x, y, w, h);
            }
            else
            {
                var window = new OverlayWindow();
                window.Show();
                window.Place(x, y, w, h);
                _overlays[screen.DeviceName] = new OverlayEntry { Window = window, Config = screen };
            }
        }

        // Close overlays for monitors no longer in the profile.
        foreach (var key in _overlays.Keys.Where(k => !wanted.Contains(k)).ToList())
        {
            _overlays[key].Window.Close();
            _overlays.Remove(key);
        }

        Refresh();

        // Overlays reassert topmost after layout settles, so re-raise the app window after.
        Application.Current?.Dispatcher.BeginInvoke(new Action(RaiseExcluded), DispatcherPriority.Background);
    }

    public void Clear()
    {
        foreach (var entry in _overlays.Values)
            entry.Window.Close();
        _overlays.Clear();
    }
}
