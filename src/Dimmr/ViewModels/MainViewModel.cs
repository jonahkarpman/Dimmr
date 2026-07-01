using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Dimmr.Infrastructure;
using Dimmr.Models;
using Dimmr.Services;

namespace Dimmr.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly DimmrController _controller;

    public ObservableCollection<ScreenRowViewModel> Screens { get; } = new();
    public ObservableCollection<string> Profiles { get; } = new();
    public IReadOnlyList<int> StartupDimOptions { get; } = new[] { 0, 10, 20, 30, 40, 50, 60, 70, 80 };

    public ICommand SaveCommand { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand IdentifyCommand { get; }

    private string _newProfileName = "";
    public string NewProfileName
    {
        get => _newProfileName;
        set => SetField(ref _newProfileName, value);
    }

    public MainViewModel(DimmrController controller)
    {
        _controller = controller;
        _controller.StateChanged += OnStateChanged;

        SaveCommand = new RelayCommand(() => _controller.SaveCurrent());
        SaveAsCommand = new RelayCommand(SaveAs);
        RefreshCommand = new RelayCommand(() =>
        {
            _controller.SyncMonitors();
            _controller.RebuildOverlays();
            RebuildScreens();
        });
        IdentifyCommand = new RelayCommand(() => ScreenIdentifier.Flash());

        ReloadProfiles();
        RebuildScreens();
    }

    // ----- master -----

    public bool MasterOn
    {
        get => _controller.Profile.MasterOn;
        set { _controller.SetMasterOn(value); OnPropertyChanged(); }
    }

    public int MasterDim
    {
        get => _controller.Profile.MasterDim;
        set
        {
            _controller.SetMasterDim(value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(MasterOn));
            foreach (var screen in Screens)
                screen.RaiseAll();
        }
    }

    // ----- profiles -----

    public string SelectedProfile
    {
        get => _controller.Profile.Name;
        set
        {
            if (!string.IsNullOrEmpty(value) && value != _controller.Profile.Name)
            {
                _controller.SwitchProfile(value);
                RebuildScreens();
                OnPropertyChanged();
                OnPropertyChanged(nameof(MasterOn));
                OnPropertyChanged(nameof(MasterDim));
            }
        }
    }

    // ----- startup / settings -----

    public bool RunAtStartup
    {
        get => _controller.Settings.RunAtStartup;
        set { _controller.SetRunAtStartup(value); OnPropertyChanged(); }
    }

    public bool ResetDimOnStartup
    {
        get => _controller.Settings.ResetDimOnStartup;
        set { _controller.Settings.ResetDimOnStartup = value; _controller.SaveSettings(); OnPropertyChanged(); }
    }

    public int StartupDim
    {
        get => _controller.Settings.StartupDim;
        set { _controller.Settings.StartupDim = value; _controller.SaveSettings(); OnPropertyChanged(); }
    }

    public bool SoundsEnabled
    {
        get => _controller.Settings.SoundsEnabled;
        set { _controller.Settings.SoundsEnabled = value; _controller.SaveSettings(); if (value) _controller.StartHum(); else _controller.StopHum(); OnPropertyChanged(); }
    }

    public bool Scanlines
    {
        get => _controller.Settings.Scanlines;
        set { _controller.Settings.Scanlines = value; _controller.SaveSettings(); OnPropertyChanged(); }
    }

    public bool Glow
    {
        get => _controller.Settings.Glow;
        set { _controller.Settings.Glow = value; _controller.SaveSettings(); OnPropertyChanged(); }
    }

    public bool Hum
    {
        get => _controller.Settings.Hum;
        set { _controller.Settings.Hum = value; _controller.SaveSettings(); if (value) _controller.StartHum(); else _controller.StopHum(); OnPropertyChanged(); }
    }

    // ----- helpers -----

    /// <summary>Full refresh after an external change (tray profile switch, show window).</summary>
    public void ExternalRefresh()
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            ReloadProfiles();
            RebuildScreens();
            OnPropertyChanged(nameof(SelectedProfile));
            OnPropertyChanged(nameof(MasterOn));
            OnPropertyChanged(nameof(MasterDim));
            OnPropertyChanged(nameof(RunAtStartup));
        });
    }

    private void SaveAs()
    {
        var name = string.IsNullOrWhiteSpace(NewProfileName) ? "profile" : NewProfileName.Trim();
        _controller.SaveAsProfile(name);
        NewProfileName = "";
        ReloadProfiles();
        RebuildScreens();
        OnPropertyChanged(nameof(SelectedProfile));
        OnPropertyChanged(nameof(MasterOn));
        OnPropertyChanged(nameof(MasterDim));
        RaiseSelectionLater();
    }

    public void DeleteSelected()
    {
        _controller.DeleteProfile(_controller.Profile.Name);
        ReloadProfiles();
        RebuildScreens();
        OnPropertyChanged(nameof(SelectedProfile));
        OnPropertyChanged(nameof(MasterOn));
        OnPropertyChanged(nameof(MasterDim));
        RaiseSelectionLater();
    }

    // Re-raise the selection after the Profiles collection settles so the ComboBox
    // reliably shows the active profile.
    private void RaiseSelectionLater()
        => Application.Current?.Dispatcher.BeginInvoke(
            new System.Action(() => OnPropertyChanged(nameof(SelectedProfile))),
            System.Windows.Threading.DispatcherPriority.Background);

    public void PlayClick() => _controller.PlayClick();
    public void PlayNavIn() => _controller.PlayNavIn();
    public void PlayNavOut() => _controller.PlayNavOut();
    public void PlayToggleTick() => _controller.PlayToggleTick();
    public void PlayKeystroke() => _controller.PlayKeystroke();
    public void StartHum() => _controller.StartHum();
    public void StopHum() => _controller.StopHum();

    private void ReloadProfiles()
    {
        Profiles.Clear();
        foreach (var name in _controller.ProfileNames())
            Profiles.Add(name);
    }

    private void RebuildScreens()
    {
        Screens.Clear();
        foreach (var screen in _controller.Profile.Screens)
            Screens.Add(new ScreenRowViewModel(screen, () => _controller.ScreenEdited()));
    }

    private void OnStateChanged()
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            OnPropertyChanged(nameof(MasterOn));
            OnPropertyChanged(nameof(MasterDim));
            foreach (var screen in Screens)
                screen.RaiseAll();
        });
    }
}
