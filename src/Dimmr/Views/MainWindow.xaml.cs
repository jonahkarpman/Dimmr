using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Dimmr.ViewModels;

namespace Dimmr.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    /// <summary>Set by the app on real exit so closing tears down instead of hiding.</summary>
    public bool ForceClose { get; set; }

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        TitleBar.MouseLeftButtonDown += OnTitleBarDrag;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Loaded += (_, _) => UpdateGlow();

        // Global UI sound wiring.
        AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnAnyButtonClick), true);
        AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(OnAnyToggle), true);
        AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(OnAnyToggle), true);
        AddHandler(Slider.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(OnAnySliderChanged), true);
        AddHandler(Selector.SelectionChangedEvent, new SelectionChangedEventHandler(OnAnySelectionChanged), true);
        PreviewTextInput += OnAnyTextInput;
        PreviewKeyDown += OnAnyKeyDown;
        Activated += (_, _) => _viewModel.StartHum();
        Deactivated += (_, _) => _viewModel.StopHum();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.Glow))
            UpdateGlow();
    }

    private void UpdateGlow()
    {
        // Respect the Windows "reduce motion" setting.
        var on = _viewModel.Glow && SystemParameters.ClientAreaAnimation;
        if (on)
        {
            var animation = new DoubleAnimation(0.55, 1.0, new Duration(TimeSpan.FromSeconds(1.8)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            TitleText.BeginAnimation(OpacityProperty, animation);
        }
        else
        {
            TitleText.BeginAnimation(OpacityProperty, null);
            TitleText.Opacity = 1.0;
        }
    }

    private void OnTitleBarDrag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnHide(object sender, RoutedEventArgs e) => Hide();

    private void OnOpenSettings(object sender, RoutedEventArgs e)
    {
        SettingsOverlay.Visibility = Visibility.Visible;
        _viewModel.PlayNavIn();
    }

    private void OnCloseSettings(object sender, RoutedEventArgs e)
    {
        SettingsOverlay.Visibility = Visibility.Collapsed;
        _viewModel.PlayNavOut();
    }

    private void OnSettingsBackdrop(object sender, MouseButtonEventArgs e)
    {
        SettingsOverlay.Visibility = Visibility.Collapsed;
        _viewModel.PlayNavOut();
    }

    private void OnSettingsPanelClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void OnDeleteProfile(object sender, RoutedEventArgs e)
    {
        _viewModel.PlayNavIn();
        var name = _viewModel.SelectedProfile;
        if (ConfirmDialog.Show(this, "CONFIRM DELETION", $"TERMINATE PROFILE: {name}?"))
            _viewModel.DeleteSelected();
        else
            _viewModel.PlayNavOut();
    }

    private void OnAnyButtonClick(object sender, RoutedEventArgs e)
    {
        if (e.Source is CheckBox)
            return; // checkboxes have their own toggle sound
        if (e.Source is Button b && (b.Tag as string) == "no-sound")
            return; // has its own specific sound
        _viewModel.PlayClick();
    }

    private void OnAnyToggle(object sender, RoutedEventArgs e)
    {
        if (e.Source is CheckBox)
            _viewModel.PlayToggleTick();
    }

    private void OnAnyTextInput(object sender, TextCompositionEventArgs e) => _viewModel.PlayKeystroke();

    // Backspace and Delete do not raise text-input events, so play the typing sound here.
    private void OnAnyKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Back || e.Key == Key.Delete)
            _viewModel.PlayKeystroke();
    }

    private DateTime _lastSliderSound = DateTime.MinValue;

    private void OnAnySliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var now = DateTime.UtcNow;
        if ((now - _lastSliderSound).TotalMilliseconds < 40)
            return;
        _lastSliderSound = now;
        _viewModel.PlaySliderTick();
    }

    private void OnAnySelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel.Repopulating)
            return;
        _viewModel.PlayClick();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!ForceClose)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnClosing(e);
    }
}
