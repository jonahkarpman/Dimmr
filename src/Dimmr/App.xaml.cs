using System;
using System.Linq;
using System.Windows;
using Dimmr.Services;
using Dimmr.ViewModels;
using Dimmr.Views;
using Drawing = System.Drawing;
using WinForms = System.Windows.Forms;

namespace Dimmr;

public partial class App : System.Windows.Application
{
    private DimmrController _controller = null!;
    private MainViewModel _viewModel = null!;
    private MainWindow _window = null!;
    private WinForms.NotifyIcon _tray = null!;
    private bool _shuttingDown;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _controller = new DimmrController();
        _controller.Start();
        Palette.Apply(_controller.Settings.ColorScheme);

        _viewModel = new MainViewModel(_controller);
        _window = new MainWindow(_viewModel);
        // Register the window so the dim overlays keep it above the dim (it is not dimmed).
        _controller.SetExcludedWindow(new System.Windows.Interop.WindowInteropHelper(_window).EnsureHandle());
        SetWindowIcon();

        SetupTray();

        var startupLaunch = e.Args.Any(a => string.Equals(a, "--startup", StringComparison.OrdinalIgnoreCase));
        if (!startupLaunch)
            ShowWindow();
    }

    private void SetupTray()
    {
        _tray = new WinForms.NotifyIcon
        {
            Icon = LoadIcon(),
            Visible = true,
            Text = "DIMMR"
        };

        _tray.MouseClick += (_, e) =>
        {
            if (e.Button == WinForms.MouseButtons.Left)
                ShowWindow();
        };

        var menu = new WinForms.ContextMenuStrip();
        menu.Opening += (_, _) => BuildMenu(menu);
        _tray.ContextMenuStrip = menu;
        BuildMenu(menu);
    }

    private void BuildMenu(WinForms.ContextMenuStrip menu)
    {
        menu.Items.Clear();

        menu.Items.Add(new WinForms.ToolStripMenuItem("Settings", null, (_, _) => ShowWindow()));

        var toggle = new WinForms.ToolStripMenuItem("Dimming on") { Checked = _controller.Settings.DimmingOn };
        toggle.Click += (_, _) =>
        {
            _controller.SetMasterOn(!_controller.Settings.DimmingOn);
            _viewModel.ExternalRefresh();
        };
        menu.Items.Add(toggle);

        var profiles = new WinForms.ToolStripMenuItem("Profiles");
        foreach (var name in _controller.ProfileNames())
        {
            var captured = name;
            var item = new WinForms.ToolStripMenuItem(name) { Checked = name == _controller.Profile.Name };
            item.Click += (_, _) =>
            {
                _controller.SwitchProfile(captured);
                _viewModel.ExternalRefresh();
            };
            profiles.DropDownItems.Add(item);
        }
        menu.Items.Add(profiles);

        var startup = new WinForms.ToolStripMenuItem("Run at startup") { Checked = _controller.Settings.RunAtStartup };
        startup.Click += (_, _) =>
        {
            _controller.SetRunAtStartup(!_controller.Settings.RunAtStartup);
            _viewModel.ExternalRefresh();
        };
        menu.Items.Add(startup);

        menu.Items.Add(new WinForms.ToolStripSeparator());
        menu.Items.Add(new WinForms.ToolStripMenuItem("Exit", null, (_, _) => ExitApp()));
    }

    private static string IconPath => System.IO.Path.Combine(AppContext.BaseDirectory, "dimmr.ico");

    private static Drawing.Icon LoadIcon()
    {
        try
        {
            if (System.IO.File.Exists(IconPath))
                return new Drawing.Icon(IconPath);
        }
        catch { /* fall back below */ }
        return Drawing.SystemIcons.Application;
    }

    private void SetWindowIcon()
    {
        try
        {
            if (System.IO.File.Exists(IconPath))
                _window.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri(IconPath));
        }
        catch { /* window keeps default icon */ }
    }

    private void ShowWindow()
    {
        _window.Show();
        _window.WindowState = WindowState.Normal;
        _window.Activate();
        _viewModel.ExternalRefresh();
    }

    private void ExitApp()
    {
        if (_shuttingDown)
            return;
        _shuttingDown = true;

        _window.ForceClose = true;
        _tray.Visible = false;
        _tray.Dispose();
        _controller.Dispose();
        Shutdown();
    }
}
