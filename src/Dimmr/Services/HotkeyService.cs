using System;
using System.Windows.Interop;
using Dimmr.Interop;

namespace Dimmr.Services;

/// <summary>
/// Registers global hotkeys on a message-only window:
/// Win+Shift+D toggles dimming, Alt+PageUp brightens, Alt+PageDown dims.
/// </summary>
public sealed class HotkeyService : IDisposable
{
    private const int IdToggle = 1;
    private const int IdBrighter = 2;
    private const int IdDimmer = 3;

    private HwndSource? _source;
    private IntPtr _hwnd;

    public event Action? ToggleRequested;
    public event Action? DimMore;
    public event Action? DimLess;

    public void Register()
    {
        var parameters = new HwndSourceParameters("DimmrHotkeys")
        {
            Width = 0,
            Height = 0,
            ParentWindow = new IntPtr(-3), // HWND_MESSAGE
        };
        _source = new HwndSource(parameters);
        _hwnd = _source.Handle;
        _source.AddHook(WndProc);

        const uint noRepeat = NativeMethods.MOD_NOREPEAT;
        NativeMethods.RegisterHotKey(_hwnd, IdToggle, NativeMethods.MOD_WIN | NativeMethods.MOD_SHIFT | noRepeat, 0x44); // D
        NativeMethods.RegisterHotKey(_hwnd, IdBrighter, NativeMethods.MOD_WIN | NativeMethods.MOD_SHIFT | noRepeat, NativeMethods.VK_NEXT);  // Page Down = less dim
        NativeMethods.RegisterHotKey(_hwnd, IdDimmer, NativeMethods.MOD_WIN | NativeMethods.MOD_SHIFT | noRepeat, NativeMethods.VK_PRIOR);   // Page Up = more dim
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == NativeMethods.WM_HOTKEY)
        {
            switch (wParam.ToInt32())
            {
                case IdToggle: ToggleRequested?.Invoke(); handled = true; break;
                case IdBrighter: DimLess?.Invoke(); handled = true; break;
                case IdDimmer: DimMore?.Invoke(); handled = true; break;
            }
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        if (_hwnd != IntPtr.Zero)
        {
            NativeMethods.UnregisterHotKey(_hwnd, IdToggle);
            NativeMethods.UnregisterHotKey(_hwnd, IdBrighter);
            NativeMethods.UnregisterHotKey(_hwnd, IdDimmer);
            _hwnd = IntPtr.Zero;
        }
        _source?.Dispose();
        _source = null;
    }
}
