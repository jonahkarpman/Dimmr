# Changelog

All notable changes to Dimmr are documented here, newest first.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this
project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.3.0] - 2026-07-01

### Changed

- Brightness hotkeys moved to Win+Shift+Page Up / Win+Shift+Page Down (Alt+Page Up/Down
  conflicted with f.lux). Toggle stays Win+Shift+D.
- Profile symbol buttons are now a uniform size, and the settings cog is a drawn gear.
- Profile layout: save sits by the dropdown; new `[+]` and delete `[-]` sit together by
  the name box, with delete in amber. Checkbox marks are filled squares, and the name box
  text is vertically centered.

### Fixed

- The profile dropdown reliably shows the active profile after creating a profile or
  reopening the window (the list is updated in place instead of cleared and rebuilt).
- Settings cog now renders a drawn gear (the font glyph showed as a missing-glyph box).
- The version in the title bar tracks the app version automatically.

## [0.2.0] - 2026-07-01

### Added

- Delete profile via a `[x]` button with a confirmation dialog ("CONFIRM DELETION",
  "[ ABORT ] / [ AFFIRM ]") and a Fallout scrap sound on confirm.
- Fallout UI sounds wired from bundled .wav files: click on buttons, target-lock on
  checkboxes, keystroke on typing, VATS in/out on opening/closing settings, start on new
  profile, scrap on delete, plus a looping ambient hum while the window is focused. All
  gated by the Sounds toggle; the hum has its own "Ambient hum" toggle.
- Sounds also on selecting a profile, opening a dropdown, and moving a slider (throttled).
- Sounds on opening the delete confirm and on aborting it (VATS in/out), in addition to the
  scrap on confirm.

### Changed

- Profile controls are now compact symbol buttons next to their fields: `[+]` new profile
  from current settings, save (floppy icon) to the selected profile, `[x]` delete.
- Brand shown in uppercase ("DIMMR") in the title bar, window title, and tray tooltip.
  The auto-created fallback profile is named "DEFAULT".

### Fixed

- Switching profiles no longer briefly turns all dimming off; overlays are reused and just
  update their opacity instead of being recreated.
- Tooltips now use the dark terminal styling (green on near-black) instead of the system
  white background.
- Creating a profile clones the current live settings instead of a blank, muted profile,
  and now reliably selects the new profile.

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
