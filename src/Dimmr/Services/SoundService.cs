using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>
/// Plays Fallout-style UI sounds via WPF MediaPlayer, one player per clip so they can
/// overlap. Files live in Resources/Sounds and are copied next to the exe. All playback
/// is gated by the Sounds setting.
/// </summary>
public sealed class SoundService : IDisposable
{
    private readonly AppSettings _settings;
    private readonly Dictionary<string, MediaPlayer> _players = new();

    public SoundService(AppSettings settings)
    {
        _settings = settings;

        var dir = Path.Combine(AppContext.BaseDirectory, "Resources", "Sounds");
        Register("click", Path.Combine(dir, "UI_Pipboy_OK_Press.wav"));
        Register("toggle", Path.Combine(dir, "UI_VATS_TargetLock_01.wav"));
        Register("adjust", Path.Combine(dir, "UI_VATS_Move.wav"));
        Register("scrap", Path.Combine(dir, "UI_WorkshopMode_Item_Scrap_Generic_01.wav"));
        Register("start", Path.Combine(dir, "UI_Start_01.wav"));
    }

    private void Register(string name, string path)
    {
        if (!File.Exists(path))
            return;
        try
        {
            var player = new MediaPlayer();
            player.Open(new Uri(path));
            _players[name] = player;
        }
        catch { /* non-critical */ }
    }

    private void Play(string name)
    {
        if (!_settings.SoundsEnabled)
            return;
        if (!_players.TryGetValue(name, out var player))
            return;
        try
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }
        catch { /* non-critical */ }
    }

    public void Toggle() => Play("toggle");
    public void Adjust() => Play("adjust");
    public void Click() => Play("click");
    public void Scrap() => Play("scrap");
    public void Start() => Play("start");

    public void Dispose()
    {
        foreach (var player in _players.Values)
        {
            try { player.Close(); } catch { /* ignore */ }
        }
        _players.Clear();
    }
}
