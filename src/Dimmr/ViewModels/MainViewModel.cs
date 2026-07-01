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
    public ICommand NewProfileCommand { get; }
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
        NewProfileCommand = new RelayCommand(NewProfile);
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
        set { _controller.SetMasterDim(value); OnPropertyChanged(); OnPropertyChanged(nameof(MasterOn)); }
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
        set { _controller.Settings.SoundsEnabled = value; _controller.SaveSettings(); OnPropertyChanged(); }
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

    private void NewProfile()
    {
        var name = string.IsNullOrWhiteSpace(NewProfileName) ? "profile" : NewProfileName.Trim();
        _controller.NewProfile(name);
        NewProfileName = "";
        ReloadProfiles();
        RebuildScreens();
        OnPropertyChanged(nameof(SelectedProfile));
        OnPropertyChanged(nameof(MasterOn));
        OnPropertyChanged(nameof(MasterDim));
    }

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
            Screens.Add(new ScreenRowViewModel(screen, () => _controller.RefreshOverlays()));
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
