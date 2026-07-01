# Dimmr — Project Specification

> Version: 0.3.3
> Last updated: 2026-07-01
> Status: Initial version built

---

## Overview

Dimmr is a Windows desktop utility that dims one or more screens using a translucent,
click-through overlay per monitor. It supports named profiles for different physical
setups, global hotkeys, and launch at startup. The interface uses a Phosphor terminal
aesthetic.

## Motivation

Some displays cannot be dimmed by the usual means. Software color tools (Night Light and
similar) and hardware brightness controls do not work on every monitor, GPU, or dock
combination, and an external display in particular can end up stuck at full brightness.

An overlay dimmer sidesteps all of that: it renders a translucent, click-through window on
top of the screen, so it works on any GPU and any monitor regardless of driver support.
Dimmr also handles docked and mixed-DPI monitors correctly, which simpler overlay dimmers
often get wrong, using PerMonitorV2 awareness plus a manual bounds override.

## Target User

- Anyone who wants per-monitor software dimming with hotkeys and profiles.
- Setups where built-in brightness or color controls do not affect a given monitor.

## Core Features

### 1. Overlay dimming
- One topmost, click-through, layered window per monitor.
- Opacity is the dim level, so input passes through and nothing is blocked.
- Positioned in physical pixels via SetWindowPos for exact coverage.

### 2. Per-screen dimming and the global switch
- Each screen has its own dim level, 0 to 85 percent (capped below 100 so a screen never
  goes fully black), plus an enable flag. Enabled screens dim; disabled ones never do.
- "Dimming on" is a single global switch (a mute), stored in settings, not per profile.
- The master slider sets every enabled screen to one level; hotkeys adjust every enabled
  screen relatively. Neither touches disabled screens.
- Effective dim = (dimming on and screen enabled) ? clamp(screen dim, 0, 85) : 0.

### 3. Bounds handling
- Each screen defaults to auto-detected bounds (PerMonitorV2 physical pixels).
- A per-screen manual override (X, Y, width, height) covers any monitor that misreads
  behind a dock.

### 4. Profiles
- Named profiles, one per setup, stored as JSON.
- Each profile holds its master state and its list of screens.
- Switch profiles from the settings window or the tray.

### 5. Hotkeys
- Win+Shift+D toggles dimming to the last level.
- Win+Shift+Page Up adds dim, Win+Shift+Page Down removes dim (5 percent steps), on the
  enabled screens only.
- Registered globally; coexists with color tools like Night Light (overlay and gamma are
  independent).

### 6. System tray
- Tray icon with: Settings, Dimming on/off, Profiles submenu, Run at startup, Exit.
- Left-click opens the settings window.

### 7. Startup
- Optional launch at login via the HKCU Run key (`--startup` argument).
- "Reset dim on startup" plus a "Start at" level. Default is undimmed (0) so hotkeys are
  armed without dimming during the day. When unticked, the last saved level is restored.

### 8. Sounds
- A sound service with hooks for toggle and adjust actions.
- Ships with system-sound placeholders; real Phosphor audio files can be added later.

### 9. Identify screens
- Flashes a large number, device name, and resolution on each monitor for two seconds, to
  confirm which physical screen a config entry maps to.

---

## Design Language

Theme: Phosphor terminal, a green-on-near-black CRT look.

### Color Palette

| Role | Hex |
|------|-----|
| Background | `#0A0A0A` |
| Surface | `#0D1A0D` |
| Primary text | `#33FF33` |
| Secondary text | `#1A8C1A` |
| Accent | `#00FF41` |
| Danger | `#FF8800` |
| Border | `#1A4D1A` |
| Disabled | `#0D3D0D` |

### Typography
- Consolas throughout, uppercase section headers.

### Components
- Bordered green buttons that fill green on hover.
- Rectangular check boxes with an `X` mark.
- Sliders with a green fill and a rectangular thumb.
- Themed dark dropdowns with a green focus glow.
- A title-bar cog that opens a centered settings panel over the dimmed app.
- Custom title bar: `> DIMMR v0.3.2_`, with `[_]` and `[X]`.
- Optional scanline overlay (toggleable).

---

## Technical Architecture

### Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 8 (net8.0-windows) |
| UI | WPF |
| Tray | System.Windows.Forms.NotifyIcon |
| Interop | Win32 P/Invoke (overlays, click-through, hotkeys, monitors) |
| Config | JSON via System.Text.Json |
| Pattern | MVVM |

### Project Structure

```
src/Dimmr/
  App.xaml / App.xaml.cs        Entry point, tray, orchestration
  app.manifest                  PerMonitorV2 DPI awareness
  Models/                       AppSettings, Profile, ScreenConfig, AppConstants
  Services/                     DimmrController, OverlayManager, MonitorService,
                                ProfileService, HotkeyService, StartupManager, SoundService
  Interop/NativeMethods.cs      Win32 declarations
  ViewModels/                   MainViewModel, ScreenRowViewModel
  Views/                        MainWindow, OverlayWindow
  Converters/                   InverseBooleanConverter
  Infrastructure/               ViewModelBase, RelayCommand
  Resources/                    Colors.xaml, Styles.xaml
```

### Configuration File Location

```
%APPDATA%\Dimmr\
  settings.json
  profiles\
    <name>.json
```

---

## Accessibility Requirements

The Phosphor look must not reduce usability. Target WCAG 2.1 AA where applicable.

- Keyboard: every control reachable and operable by keyboard, logical tab order, visible
  green focus outline, Escape and Enter behave conventionally.
- Screen readers: `AutomationProperties.Name` on interactive controls; a logical automation
  tree; no meaning by color alone.
- Contrast: primary text `#33FF33` on `#0A0A0A` is about 9.5:1; secondary `#1A8C1A` is about
  4.6:1; danger `#FF8800` is about 6.4:1.
- Visual comfort: scanlines are toggleable; no flicker by default; respect the Windows
  Reduce Motion and High Contrast settings.
- Targets: interactive controls at least 32 by 32, adequate spacing.

### Implementation Checklist
- [x] AutomationProperties.Name on interactive controls
- [x] Keyboard-operable controls and focus visuals
- [x] Contrast-safe palette
- [x] Scanlines toggle
- [ ] Verify tab order on every view
- [ ] Test with Windows Narrator
- [ ] Honor SystemParameters.HighContrast fallback
- [ ] Honor Reduce Motion for any animation
- [ ] Test at 150 percent DPI across mixed-DPI monitors

---

## Roadmap

### Phase 1 — MVP (done)
- [x] Overlay engine with click-through and exact placement
- [x] Master and per-screen control
- [x] Profiles (JSON load/save)
- [x] Global hotkeys
- [x] System tray
- [x] Startup registration and startup-dim behavior
- [x] Phosphor theme and base accessibility

### Phase 2 — Polish
- [ ] Real Phosphor sound effects
- [ ] Segmented (health-bar) slider styling
- [ ] Visual monitor preview for editing bounds
- [ ] Custom tray icon
- [ ] Scanline overlay rendered in the window
- [ ] High Contrast and Reduce Motion handling

### Phase 3 — Advanced
- [ ] Per-monitor tint (not just black)
- [ ] Schedule-based auto dim (time of day)
- [ ] Auto profile selection by connected monitors
- [ ] Packaged single-file build

---

## Open Questions

1. Should dimming ever cover exclusive full-screen games, or explicitly skip them?
2. Should profiles auto-select based on the connected monitors?
3. Package as a single-file exe, or keep the framework-dependent build?
