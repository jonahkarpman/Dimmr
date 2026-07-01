using System;
using System.Windows;
using System.Windows.Interop;
using Dimmr.Interop;

namespace Dimmr.Views;

/// <summary>
/// A single click-through, topmost black overlay covering one monitor. Positioned in
/// physical pixels via SetWindowPos so it covers docked and mixed-DPI screens fully.
/// </summary>
public partial class OverlayWindow : Window
{
    private int _x, _y, _w, _h;

    public OverlayWindow()
    {
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;
    }

    public void Place(int x, int y, int w, int h)
    {
        _x = x;
        _y = y;
        _w = w;
        _h = h;
        if (IsLoaded || new WindowInteropHelper(this).Handle != IntPtr.Zero)
            ApplyPlacement();
    }

    /// <summary>Sets dim as a 0..1 fraction. Hidden entirely when zero.</summary>
    public void SetDim(double fraction)
    {
        var clamped = Math.Clamp(fraction, 0.0, 0.95);
        Opacity = clamped;
        Visibility = clamped <= 0.0 ? Visibility.Hidden : Visibility.Visible;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        MakeClickThrough();
        ApplyPlacement();
    }

    private void MakeClickThrough()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd == IntPtr.Zero)
            return;

        long ex = NativeMethods.GetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE).ToInt64();
        ex |= NativeMethods.WS_EX_LAYERED
              | NativeMethods.WS_EX_TRANSPARENT
              | NativeMethods.WS_EX_TOOLWINDOW
              | NativeMethods.WS_EX_NOACTIVATE;
        NativeMethods.SetWindowLongPtr(hwnd, NativeMethods.GWL_EXSTYLE, new IntPtr(ex));
    }

    private void ApplyPlacement()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        if (hwnd == IntPtr.Zero || _w <= 0 || _h <= 0)
            return;

        NativeMethods.SetWindowPos(
            hwnd,
            NativeMethods.HWND_TOPMOST,
            _x, _y, _w, _h,
            NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
    }
}
