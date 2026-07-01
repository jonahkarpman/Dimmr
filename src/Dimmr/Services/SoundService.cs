using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>
/// Plays Fallout-style UI sounds via WPF MediaPlayer, one player per clip so they can
/// overlap. Includes a looping ambient hum. Files live in Resources/Sounds next to the
/// exe. One-shot sounds are gated by the Sounds setting; the hum also needs Hum on.
/// </summary>
public sealed class SoundService : IDisposable
{
    private readonly AppSettings _settings;
    private readonly Dictionary<string, MediaPlayer> _players = new();
    private MediaPlayer? _hum;
    private bool _humPlaying;

    public SoundService(AppSettings settings)
    {
        _settings = settings;

        var dir = Path.Combine(AppContext.BaseDirectory, "Resources", "Sounds");
        Register("click", Path.Combine(dir, "UI_Pipboy_OK_Press.wav"));
        Register("toggle", Path.Combine(dir, "UI_VATS_TargetLock_01.wav"));
        Register("adjust", Path.Combine(dir, "UI_VATS_Move.wav"));
        Register("scrap", Path.Combine(dir, "UI_WorkshopMode_Item_Scrap_Generic_01.wav"));
        Register("start", Path.Combine(dir, "UI_Start_01.wav"));
        Register("navin", Path.Combine(dir, "UI_VATS_Enter.wav"));
        Register("navout", Path.Combine(dir, "UI_VATS_Exit.wav"));
        Register("keystroke", Path.Combine(dir, "UI_Hacking_CharSingle_01.wav"));

        var humPath = Path.Combine(dir, "UI_PipBoy_Hum_LP.wav");
        if (File.Exists(humPath))
        {
            try
            {
                _hum = new MediaPlayer();
                _hum.Open(new Uri(humPath));
                _hum.MediaEnded += (_, _) =>
                {
                    if (_humPlaying && _hum != null)
                    {
                        _hum.Position = TimeSpan.Zero;
                        _hum.Play();
                    }
                };
            }
            catch { _hum = null; }
        }
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
            player.Stop();
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
    public void NavIn() => Play("navin");
    public void NavOut() => Play("navout");
    public void ToggleTick() => Play("toggle");
    public void Keystroke() => Play("keystroke");

    public void StartHum()
    {
        if (_hum == null || _humPlaying)
            return;
        if (!_settings.SoundsEnabled || !_settings.Hum)
            return;
        try
        {
            _hum.Position = TimeSpan.Zero;
            _hum.Play();
            _humPlaying = true;
        }
        catch { /* non-critical */ }
    }

    public void StopHum()
    {
        if (!_humPlaying || _hum == null)
            return;
        try { _hum.Stop(); } catch { /* ignore */ }
        _humPlaying = false;
    }

    public void Dispose()
    {
        StopHum();
        _hum?.Close();
        foreach (var player in _players.Values)
        {
            try { player.Close(); } catch { /* ignore */ }
        }
        _players.Clear();
    }
}
