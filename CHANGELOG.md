# Changelog

All notable changes to Dimmr are documented here, newest first.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this
project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.1.0] - 2026-06-30

Initial working version.

### Added

- Click-through, topmost overlay per monitor for dimming without blocking input.
- Master dim level plus per-screen enable and offset.
- PerMonitorV2 DPI awareness with a manual per-screen bounds override, so overlays cover
  docked and mixed-DPI displays fully.
- Named profiles saved as JSON under `%APPDATA%\Dimmr`, switchable from the window or tray.
- Global hotkeys: Win+Shift+D to toggle, Alt+Page Up and Alt+Page Down to adjust.
- System tray icon with settings, dimming toggle, profile switch, startup toggle, and exit.
- Launch at startup via the HKCU Run key, with a "reset dim on startup" option and a
  configurable startup level (defaults to undimmed).
- Phosphor terminal theme (green on near-black) with keyboard navigation, focus
  visuals, screen-reader names, and a scanlines toggle.
- Placeholder sound hooks (system sounds) ready for real audio files later.
