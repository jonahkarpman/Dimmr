using System.Media;
using Dimmr.Models;

namespace Dimmr.Services;

/// <summary>
/// Plays small feedback sounds for actions. Uses system sounds as placeholders; drop
/// real Phosphor .wav files into a Sounds folder later and wire them here.
/// </summary>
public sealed class SoundService
{
    private readonly AppSettings _settings;

    public SoundService(AppSettings settings) => _settings = settings;

    public void Toggle()
    {
        if (_settings.SoundsEnabled)
            SystemSounds.Hand.Play();
    }

    public void Adjust()
    {
        if (_settings.SoundsEnabled)
            SystemSounds.Beep.Play();
    }
}
