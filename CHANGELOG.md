# Changelog

All notable changes to Dimmr are documented here, newest first.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this
project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Fixed

- Creating a profile now clones the current live settings under the new name (Save As) and
  selects it, instead of making a blank, muted profile. The button is labelled "Save As".

## [0.1.0] - 2026-06-30

Initial working version.

### Added

- Click-through, topmost overlay per monitor for dimming without blocking input.
- Master dim as a broadcast control, plus an independent per-screen dim level
  (0 to 85 percent) and an enable toggle per screen.
- PerMonitorV2 DPI awareness with a manual per-screen bounds override, and reassertion of
  the exact physical rect on DPI change, so overlays cover docked and mixed-DPI displays
  fully.
- Identify Screens: flashes a number and resolution on each monitor to confirm targeting.
- Named profiles saved as JSON under `%APPDATA%\Dimmr`, switchable from the window or tray.
- Global hotkeys: Win+Shift+D to toggle, Alt+Page Up and Alt+Page Down to adjust.
- System tray icon with settings, dimming toggle, profile switch, startup toggle, and exit.
- Launch at startup via the HKCU Run key, with a "reset dim on startup" option and a
  configurable startup level (defaults to undimmed); the startup path follows the running
  copy.
- Phosphor terminal theme (green on near-black): a settings panel behind a title-bar cog,
  themed dark dropdowns, a breathing title glow, keyboard navigation, focus visuals,
  screen-reader names, and a scanlines toggle.
- Phosphor brightness-glyph icon for the app, tray, and window.
- Placeholder sound hooks (system sounds) ready for real audio files later.
