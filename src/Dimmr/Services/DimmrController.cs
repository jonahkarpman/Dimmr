using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>
/// Central coordinator: owns settings, the active profile, overlays, hotkeys and sounds,
/// and exposes the operations the UI and hotkeys drive.
/// </summary>
public sealed class DimmrController : IDisposable
{
    private readonly ProfileService _profiles;
    private readonly OverlayManager _overlays = new();
    private readonly HotkeyService _hotkeys = new();
    private readonly SoundService _sounds;

    public AppSettings Settings { get; }
    public Profile Profile { get; private set; }

    /// <summary>Raised when state changes from outside the UI (hotkeys, display changes).</summary>
    public event Action? StateChanged;

    public DimmrController()
    {
        _profiles = new ProfileService();
        Settings = _profiles.LoadSettings();
        _sounds = new SoundService(Settings);
        Profile = _profiles.LoadProfile(Settings.ActiveProfile);
    }

    public void Start()
    {
        SyncMonitors();
        ApplyStartupDim();
        _overlays.Apply(Profile);

        _hotkeys.ToggleRequested += ToggleMaster;
        _hotkeys.DimMore += () => AdjustMaster(+AppConstants.DimStep);
        _hotkeys.DimLess += () => AdjustMaster(-AppConstants.DimStep);
        _hotkeys.Register();

        SystemEvents.DisplaySettingsChanged += OnDisplaysChanged;

        // Keep the startup entry pointed at whichever copy is actually running.
        if (Settings.RunAtStartup)
            StartupManager.SetEnabled(true);
    }

    private void ApplyStartupDim()
    {
        if (!Settings.ResetDimOnStartup)
            return;

        Profile.MasterDim = Math.Clamp(Settings.StartupDim, 0, AppConstants.MaxDim);
        Profile.MasterOn = Settings.StartupDim > 0;
        foreach (var screen in Profile.Screens)
            screen.Dim = Profile.MasterDim;
    }

    /// <summary>Adds a screen entry for any connected monitor the profile does not know yet.</summary>
    public void SyncMonitors()
    {
        var monitors = MonitorService.GetMonitors();
        int index = 1;
        foreach (var mon in monitors)
        {
            var existing = Profile.Screens.FirstOrDefault(s => s.DeviceName == mon.DeviceName);
            if (existing is null)
            {
                Profile.Screens.Add(new ScreenConfig
                {
                    DeviceName = mon.DeviceName,
                    Label = $"Screen {index} ({mon.Width}x{mon.Height})",
                    Enabled = true,
                    Dim = Profile.MasterDim,
                    AutoBounds = true,
                    X = mon.X,
                    Y = mon.Y,
                    Width = mon.Width,
                    Height = mon.Height
                });
            }
            else if (existing.AutoBounds)
            {
                // keep detected bounds fresh for auto screens
                existing.X = mon.X;
                existing.Y = mon.Y;
                existing.Width = mon.Width;
                existing.Height = mon.Height;
            }
            index++;
        }
    }

    // ----- operations the UI calls -----

    public void SetMasterOn(bool on)
    {
        Profile.MasterOn = on;
        _overlays.Refresh();
    }

    public void SetMasterDim(int dim)
    {
        Profile.MasterDim = Math.Clamp(dim, 0, AppConstants.MaxDim);
        Profile.MasterOn = Profile.MasterDim > 0;
        foreach (var screen in Profile.Screens)
            screen.Dim = Profile.MasterDim;
        _overlays.Refresh();
    }

    /// <summary>Called after editing one screen's dim; unmutes so the change is visible.</summary>
    public void ScreenEdited()
    {
        Profile.MasterOn = true;
        _overlays.Refresh();
    }

    public void RefreshOverlays() => _overlays.Refresh();

    public void RebuildOverlays() => _overlays.Apply(Profile);

    public void SetRunAtStartup(bool enabled)
    {
        Settings.RunAtStartup = enabled;
        StartupManager.SetEnabled(enabled);
        SaveSettings();
    }

    public void SaveSettings() => _profiles.SaveSettings(Settings);

    public void SaveCurrent()
    {
        _profiles.SaveProfile(Profile);
        Settings.ActiveProfile = Profile.Name;
        SaveSettings();
    }

    public System.Collections.Generic.List<string> ProfileNames()
    {
        var names = _profiles.ListProfileNames();
        if (!names.Contains(Profile.Name))
            names.Insert(0, Profile.Name);
        return names;
    }

    public void SwitchProfile(string name)
    {
        SaveCurrent();
        Profile = _profiles.LoadProfile(name);
        Settings.ActiveProfile = name;
        SyncMonitors();
        SaveSettings();
        _overlays.Apply(Profile);
        StateChanged?.Invoke();
    }

    public void SaveAsProfile(string name)
    {
        // Persist the current profile, then clone its live state under a new name.
        _profiles.SaveProfile(Profile);

        Profile = new Profile
        {
            Name = name,
            MasterOn = Profile.MasterOn,
            MasterDim = Profile.MasterDim,
            Screens = Profile.Screens.Select(s => new ScreenConfig
            {
                DeviceName = s.DeviceName,
                Label = s.Label,
                Enabled = s.Enabled,
                Dim = s.Dim,
                AutoBounds = s.AutoBounds,
                X = s.X,
                Y = s.Y,
                Width = s.Width,
                Height = s.Height
            }).ToList()
        };
        Settings.ActiveProfile = name;
        SyncMonitors();
        SaveCurrent();
        _overlays.Apply(Profile);
        StateChanged?.Invoke();
    }

    // ----- hotkey handlers -----

    private void ToggleMaster()
    {
        Profile.MasterOn = !Profile.MasterOn;
        _overlays.Refresh();
        _sounds.Toggle();
        StateChanged?.Invoke();
    }

    private void AdjustMaster(int delta)
    {
        Profile.MasterOn = true;
        Profile.MasterDim = Math.Clamp(Profile.MasterDim + delta, 0, AppConstants.MaxDim);
        _overlays.Refresh();
        _sounds.Adjust();
        StateChanged?.Invoke();
    }

    private void OnDisplaysChanged(object? sender, EventArgs e)
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            SyncMonitors();
            _overlays.Apply(Profile);
            StateChanged?.Invoke();
        });
    }

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaysChanged;
        try { SaveCurrent(); } catch { /* best effort on shutdown */ }
        _overlays.Clear();
        _hotkeys.Dispose();
    }
}
