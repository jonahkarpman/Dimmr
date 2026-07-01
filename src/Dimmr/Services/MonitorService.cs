using System;
using System.Collections.Generic;
using Dimmr.Interop;

namespace Dimmr.Services;

/// <summary>A physical monitor detected at runtime, in physical pixels.</summary>
public sealed record DetectedMonitor(string DeviceName, int X, int Y, int Width, int Height);

/// <summary>Enumerates connected monitors using Win32 so bounds are true physical pixels.</summary>
public static class MonitorService
{
    public static List<DetectedMonitor> GetMonitors()
    {
        var result = new List<DetectedMonitor>();

        bool Callback(IntPtr hMonitor, IntPtr hdc, ref NativeMethods.RECT rect, IntPtr data)
        {
            var info = new NativeMethods.MONITORINFOEX
            {
                cbSize = System.Runtime.InteropServices.Marshal.SizeOf<NativeMethods.MONITORINFOEX>()
            };

            if (NativeMethods.GetMonitorInfo(hMonitor, ref info))
            {
                var m = info.rcMonitor;
                result.Add(new DetectedMonitor(
                    info.szDevice,
                    m.Left,
                    m.Top,
                    m.Right - m.Left,
                    m.Bottom - m.Top));
            }

            return true;
        }

        NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Callback, IntPtr.Zero);
        return result;
    }
}
