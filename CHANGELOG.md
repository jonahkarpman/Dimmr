# Changelog

All notable changes to Dimmr are documented here, newest first.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this
project uses [Semantic Versioning](https://semver.org/).

## [Unreleased]

## [0.4.1] - 2026-07-01

### Fixed

- Selecting a color now actually applies. The palette brushes were frozen, so recoloring
  them in place did nothing; the themed brushes are now dynamic resources that get swapped
  on selection.

### Changed

- The color swatches are now small squares without text labels; the color speaks for
  itself.

## [0.4.0] - 2026-07-01

### Added

- A COLOR picker in Settings with two clickable swatches: Phosphor Green (default) and
  New Vegas Amber (`#FFB642`). Selecting one recolors the whole interface live, including
  the glow. The choice is saved. In the amber scheme the destructive controls use a hot
  amber (`#FFE0A0`) so they stay distinct.

## [0.3.7] - 2026-07-01

### Fixed

- Fixed a crash on launch introduced in 0.3.6: the glow could not animate the screen-box
  effect because template effects are frozen. The effect is now cloned before animating.

## [0.3.6] - 2026-07-01

### Changed

- The breathing glow now also pulses the section headings and casts a soft green glow on
  the per-screen boxes, not just the title. Still respects the Reduce Motion setting.

### Fixed

- The top-right close button `[X]` is now amber, matching the other destructive controls
  (it had been rendering green).

## [0.3.5] - 2026-07-01

### Fixed

- Scanlines now actually render as thin horizontal lines. The tile had no viewbox, so the
  dark line was stretched over the whole cell and just darkened the window uniformly.

## [0.3.4] - 2026-07-01

### Fixed

- Backspace and Delete now play the typing sound like other keys did (they don't raise a
  text-input event, so they were previously silent).

## [0.3.3] - 2026-07-01

### Changed

- Renamed the bundled UI sound files to generic, role-based names (`click.wav`,
  `toggle.wav`, `hum.wav`, and so on) and removed the unused clips. No change to how any
  sound plays.

## [0.3.2] - 2026-07-01

### Added

- The Scanlines option now actually renders a faint CRT scanline overlay (it was a no-op
  toggle before).

### Changed

- Newly detected monitors default to 0% dim (were 30%), so a plugged-in display is not
  dimmed until you set it.

## [0.3.1] - 2026-07-01

### Changed

- Removed the master dim slider from the window; the per-screen sliders are the dim
  control, and "Dimming on" is just the global on/off. Editing a screen no longer flips
  the global switch.

### Fixed

- Brightness hotkeys were reversed: Win+Shift+Page Up now adds dim, Page Down removes it.

## [0.3.0] - 2026-07-01

### Changed

- Dimming is now purely per-screen: a screen dims only if it is enabled, and per-screen
  enable persists per profile. "Dimming on" is a single global switch (stored in settings)
  rather than a per-profile master, so it no longer resets when you switch profiles.
- Brightness hotkeys and the master slider only affect enabled screens; disabled screens
  are never changed.

## [0.2.2] - 2026-07-01

### Fixed

- Brightness hotkeys (Win+Shift+Page Up / Down) now actually change the dim level; they
  were adjusting the master value without applying it to the screens.
- The delete button is now amber (it had been rendering green because the global text
  style overrode the button's colour).

## [0.2.1] - 2026-07-01

### Changed

- Brightness hotkeys moved to Win+Shift+Page Up / Win+Shift+Page Down (Alt+Page Up/Down
  conflicted with other apps). Toggle stays Win+Shift+D.
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
  "[ ABORT ] / [ AFFIRM ]") and a scrap sound on confirm.
- Retro terminal UI sounds wired from bundled .wav files: click on buttons, a lock tone on
  checkboxes, keystroke on typing, in/out tones on opening/closing settings, a start tone on
  new profile, scrap on delete, plus a looping ambient hum while the window is focused. All
  gated by the Sounds toggle; the hum has its own "Ambient hum" toggle.
- Sounds also on selecting a profile, opening a dropdown, and moving a slider (throttled).
- Sounds on opening the delete confirm and on aborting it (in/out tones), in addition to the
  scrap on delete confirm.

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
