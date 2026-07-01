using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Dimmr.ViewModels;

namespace Dimmr.Views;

public partial class MainWindow : Window
{
    /// <summary>Set by the app on real exit so closing tears down instead of hiding.</summary>
    public bool ForceClose { get; set; }

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        TitleBar.MouseLeftButtonDown += OnTitleBarDrag;
    }

    private void OnTitleBarDrag(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnHide(object sender, RoutedEventArgs e) => Hide();

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
