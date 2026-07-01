using System;
using System.Windows;
using System.Windows.Interop;
using Dimmr.Interop;

namespace Dimmr.Views;

/// <summary>A brief, click-through label shown on a monitor so you can confirm targeting.</summary>
public partial class IdentifyWindow : Window
{
    private int _x, _y, _w, _h;

    public IdentifyWindow()
    {
        InitializeComponent();
        SourceInitialized += (_, _) =>
        {
            MakeClickThrough();
            ApplyPlacement();
        };
        Loaded += (_, _) => ApplyPlacement();
    }

    protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
    {
        base.OnDpiChanged(oldDpi, newDpi);
        ApplyPlacement();
    }

    public void Configure(string number, string label, int x, int y, int w, int h)
    {
        NumberText.Text = number;
        LabelText.Text = label;
        _x = x;
        _y = y;
        _w = w;
        _h = h;
        ApplyPlacement();
        // Reassert after layout settles, in case WPF resized on a DPI change during show.
        Dispatcher.BeginInvoke(new Action(ApplyPlacement), System.Windows.Threading.DispatcherPriority.Loaded);
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
