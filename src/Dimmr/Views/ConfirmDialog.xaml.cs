using System.Windows;

namespace Dimmr.Views;

/// <summary>Phosphor-themed confirmation dialog. Returns true when the user affirms.</summary>
public partial class ConfirmDialog : Window
{
    public ConfirmDialog(string title, string message)
    {
        InitializeComponent();
        TitleText.Text = title;
        MessageText.Text = message;
    }

    public static bool Show(Window owner, string title, string message)
    {
        var dialog = new ConfirmDialog(title, message) { Owner = owner };
        return dialog.ShowDialog() == true;
    }

    private void OnConfirm(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
