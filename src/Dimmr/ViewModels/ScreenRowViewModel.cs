using System;
using Dimmr.Infrastructure;
using Dimmr.Models;

namespace Dimmr.ViewModels;

/// <summary>Editable row for one screen in the settings UI.</summary>
public sealed class ScreenRowViewModel : ViewModelBase
{
    private readonly ScreenConfig _model;
    private readonly Action _changed;

    public ScreenRowViewModel(ScreenConfig model, Action changed)
    {
        _model = model;
        _changed = changed;
    }

    public string Label => string.IsNullOrWhiteSpace(_model.Label) ? _model.DeviceName : _model.Label;

    public bool Enabled
    {
        get => _model.Enabled;
        set { if (_model.Enabled != value) { _model.Enabled = value; OnPropertyChanged(); _changed(); } }
    }

    public int Offset
    {
        get => _model.Offset;
        set { var v = Math.Clamp(value, -50, 50); if (_model.Offset != v) { _model.Offset = v; OnPropertyChanged(); _changed(); } }
    }

    public bool AutoBounds
    {
        get => _model.AutoBounds;
        set { if (_model.AutoBounds != value) { _model.AutoBounds = value; OnPropertyChanged(); _changed(); } }
    }

    public int X
    {
        get => _model.X;
        set { if (_model.X != value) { _model.X = value; OnPropertyChanged(); _changed(); } }
    }

    public int Y
    {
        get => _model.Y;
        set { if (_model.Y != value) { _model.Y = value; OnPropertyChanged(); _changed(); } }
    }

    public int Width
    {
        get => _model.Width;
        set { if (_model.Width != value) { _model.Width = value; OnPropertyChanged(); _changed(); } }
    }

    public int Height
    {
        get => _model.Height;
        set { if (_model.Height != value) { _model.Height = value; OnPropertyChanged(); _changed(); } }
    }

    public void RaiseAll()
    {
        OnPropertyChanged(nameof(Enabled));
        OnPropertyChanged(nameof(Offset));
        OnPropertyChanged(nameof(AutoBounds));
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(Y));
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
    }
}
