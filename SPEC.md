# Dimmr — Project Specification

> Version: 0.2.2
> Last updated: 2026-07-01
> Status: Initial version built

---

## Overview

Dimmr is a Windows desktop utility that dims one or more screens using a translucent,
click-through overlay per monitor. It supports named profiles for different physical
setups, global hotkeys, and launch at startup. The interface uses a Phosphor terminal
aesthetic.

## Problem Statement

On Snapdragon (ARM64) laptops, the Qualcomm Adreno display driver does not apply the
color/gamma pipeline or DDC/CI to external monitors. As a result:

- f.lux and Windows Night Light do nothing on an external display.
- Hardware brightness tools (Twinkle Tray, Monitorian) report the display as unsupported.

The same monitor and dock work correctly on an x86 laptop, so the gap is the ARM GPU
driver. An overlay dimmer avoids both the gamma pipeline and DDC/CI by simply rendering a
window, which works on any GPU, including Adreno.

A second, related problem: existing overlay dimmers (for example Nelson Pires' Dimmer) are
not per-monitor DPI aware and misread monitor bounds behind docks, leaving part of an
ultrawide screen uncovered. Dimmr fixes this with PerMonitorV2 awareness plus a manual
bounds override.

## Target User

- People on Snapdragon/ARM Windows laptops driving external monitors through a dock.
- Anyone who wants per-monitor software dimming with hotkeys and profiles.

## Core Features

### 1. Overlay dimming
- One topmost, click-through, layered window per monitor.
- Opacity is the dim level, so input passes through and nothing is blocked.
- Positioned in physical pixels via SetWindowPos for exact coverage.

### 2. Master and per-screen control
- Each screen has its own dim level, 0 to 85 percent (capped below 100 so a screen never
  goes fully black), plus an enable flag.
- The master switch mutes or unmutes all screens. The master slider is a broadcast: moving
  it sets every enabled screen to that level and unmutes.
- Editing a single screen's slider unmutes so the change is visible immediately.
- Effective dim = (master on and screen enabled) ? clamp(screen dim, 0, 85) : 0.

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
- Win+Shift+Page Up brightens, Win+Shift+Page Down dims (5 percent steps).
- Registered globally; coexists with f.lux (overlay vs gamma are independent).

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

Theme: Phosphor terminal, matching the sibling `windowr` project.

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
- Custom title bar: `> DIMMR v0.2.2_`, with `[_]` and `[X]`.
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
2. Should profiles auto-select based on connected monitors (like windowr does)?
3. Package as a single-file exe, or keep the framework-dependent build?
