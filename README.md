# Dimmr

> A phosphor CRT themed multi-monitor screen dimmer for Windows

Dimmr dims your screens with a click-through overlay, per monitor, with profiles and
global hotkeys. It was built to solve a specific problem: on Snapdragon (ARM64) laptops,
the Qualcomm Adreno driver does not apply the color/gamma pipeline or DDC/CI to external
monitors, so f.lux, Windows Night Light, and hardware brightness tools cannot dim an
external display. Dimmr sidesteps all of that by drawing a translucent layer, which works
on any GPU.

## Features

- Per-monitor dimming with a master control plus an independent per-screen dim level
- Identify Screens button to confirm which physical monitor is which
- Named profiles for different setups (for example `desk-g9`, `dual-external`)
- Correct coverage on docked and mixed-DPI displays (PerMonitorV2 aware, with a manual
  bounds override for any monitor that still misreads)
- Global hotkeys, works alongside f.lux
- Runs in the system tray, optional launch at startup
- Choose whether to start undimmed or at a set level
- Phosphor terminal aesthetic, with accessibility built in

## Requirements

- Windows 10 or 11
- .NET 8 Desktop Runtime (to run) or the .NET 8 SDK (to build)

## Build and run

```powershell
dotnet build
dotnet run --project src/Dimmr
```

## Usage

1. Launch Dimmr. It shows the settings window and adds a tray icon.
2. Turn on Master dimming and set the level, or use the hotkeys.
3. Adjust per-screen offsets if one monitor should be darker or lighter.
4. Save the current setup as a profile. Switch profiles from the window or the tray.
5. If a screen is not fully covered, untick "Auto-detect bounds" for it and enter the
   exact X, Y, width, height.

### Hotkeys

| Hotkey | Action |
|--------|--------|
| Win+Shift+D | Toggle dimming on/off to the last level |
| Win+Shift+Page Up | Brighter (less dim) |
| Win+Shift+Page Down | Dimmer (more dim) |

### Startup behavior

Enable "Run at startup" to launch with Windows. "Reset dim on startup" plus the "Start at"
value control the level on launch. Leave it at 0 to start undimmed so the hotkeys are armed
without dimming during the day. Untick "Reset dim on startup" to restore the last level
instead.

## Configuration

Settings and profiles are stored as JSON under `%APPDATA%\Dimmr`:

```
%APPDATA%\Dimmr\
  settings.json
  profiles\
    default.json
```

## Accessibility

Dimmr follows the accessibility rules in [SPEC.md](SPEC.md): full keyboard operation,
visible focus, screen-reader names, contrast-safe colors, and a scanlines toggle for
visual sensitivity. Nothing is conveyed by color alone.

## Documentation

See [SPEC.md](SPEC.md) for the full specification and [CHANGELOG.md](CHANGELOG.md) for the
version history.

## License

MIT
