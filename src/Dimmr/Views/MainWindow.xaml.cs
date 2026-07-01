using System;
using System.ComponentModel;
using System.Windows;
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

    private void OnOpenSettings(object sender, RoutedEventArgs e) => SettingsOverlay.Visibility = Visibility.Visible;

    private void OnCloseSettings(object sender, RoutedEventArgs e) => SettingsOverlay.Visibility = Visibility.Collapsed;

    private void OnSettingsBackdrop(object sender, MouseButtonEventArgs e) => SettingsOverlay.Visibility = Visibility.Collapsed;

    private void OnSettingsPanelClick(object sender, MouseButtonEventArgs e) => e.Handled = true;

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
