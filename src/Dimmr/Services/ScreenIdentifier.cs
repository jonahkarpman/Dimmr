using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Dimmr.Views;

namespace Dimmr.Services;

/// <summary>
/// Flashes a large number and device label on each monitor for a couple of seconds,
/// so you can confirm which physical screen a config entry maps to.
/// </summary>
public static class ScreenIdentifier
{
    private static readonly List<IdentifyWindow> Open = new();
    private static DispatcherTimer? _timer;

    public static void Flash()
    {
        Clear();

        var monitors = MonitorService.GetMonitors();
        int index = 1;
        foreach (var m in monitors)
        {
            var window = new IdentifyWindow();
            window.Show();
            window.Configure(index.ToString(), $"{m.DeviceName}   {m.Width} x {m.Height}", m.X, m.Y, m.Width, m.Height);
            Open.Add(window);
            index++;
        }

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _timer.Tick += (_, _) => Clear();
        _timer.Start();
    }

    private static void Clear()
    {
        _timer?.Stop();
        _timer = null;
        foreach (var window in Open)
            window.Close();
        Open.Clear();
    }
}
