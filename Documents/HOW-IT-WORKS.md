# How ShortCutTool Works

A short tour of the moving parts, for contributors. User-facing docs are in the [README](../README.md).

## Startup (`ShortCutTool/Program.cs`)

1. Takes the `ShortCutTool.SingleInstance` mutex. A second copy waits up to 5 seconds (so **Save & Restart** can hand over), then exits with a reminder.
2. Loads `%APPDATA%\ShortCutTool\shortcuts.json` through `ConfigStore`, creating an empty file or migrating one found next to the exe (`AppPaths.LegacyConfigFile`).
3. Validates it with `ConfigValidator` and registers each shortcut with `KeyboardHookService`. The hook gets copies with environment variables expanded; the tray keeps the originals, so saving preserves `%USERNAME%`-style paths.
4. Runs `TrayApplicationContext` until **Exit**, sign-out or an installer closes it (`SystemEvents.SessionEnded`).

## Catching the shortcut (`KeyboardHookService`, `ModifierKeyState`)

- A **low-level keyboard hook** (`SetWindowsHookEx(WH_KEYBOARD_LL)`) sees every key press system-wide.
- `ModifierKeyState` tracks Ctrl, Alt, Shift and both Windows keys. **Meh** is Ctrl+Alt+Shift without Win; **Hyper** adds Win.
- When a registered letter or digit arrives with Meh (forward) or Hyper (reverse, if `useHyperForReverse`), the hook returns `1` to swallow the key so the focused app and Windows never see it; everything else goes to `CallNextHookEx`.
- Windows does not deliver input from elevated windows to a non-elevated hook (UIPI), so shortcuts don't fire while an admin window has focus unless ShortCutTool also runs elevated.

## Launch or cycle (`KeyboardHookService.HandleShortcut`, `WindowEnumerator`)

Windows are matched by **process name** (the exe file name without `.exe`):

| Matching windows | Action |
|---|---|
| 0 | Launch `applicationPath` (shell execute, optional working directory) |
| 1 | Restore it if minimised and bring it to the front |
| 2+ | Move the per-key index forward (Meh) or backward (Hyper), wrapping around, activate that window, and show `CyclePopup` with the window list |

`WindowEnumerator` walks all top-level windows with `EnumWindows` and keeps the visible, titled ones whose owning process has that name. `Process.MainWindowHandle` is not enough: it reports at most one window per process, and multi-process apps such as VS Code, Chrome and Edge spread their windows across many processes.

Bringing a window forward uses `AttachThreadInput` plus `ShowWindow`/`SetForegroundWindow`, because Windows' foreground-lock rules otherwise make a background process's `SetForegroundWindow` call flash the taskbar instead of switching.

`CyclePopup` appears near the system tray and stays until the modifier keys are released.

## Configuration safety (`ConfigValidator`)

`shortcuts.json` is user-editable, so every shortcut is checked before use: single A-Z/0-9 key, no duplicate keys, at most 100 shortcuts, absolute (or `%VAR%`-based) paths up to 500 characters, and no `..`, UNC paths, control or reserved characters, or device names. The rules are covered by `tests/ShortCutTool.Tests/ConfigValidatorTests.cs`.

## Files on disk

| Path | Written by |
|---|---|
| `%LOCALAPPDATA%\Programs\ShortCutTool\` | Installer (app files) |
| `%APPDATA%\ShortCutTool\shortcuts.json` | App (`ConfigStore`) |
| `%LOCALAPPDATA%\ShortCutTool\logs\shortcuttool.log` (+ `.1.log`) | App (`Log`), rolls at 1 MB |
| `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` → `ShortCutTool` | Tray toggle and installer task; removed on uninstall |
