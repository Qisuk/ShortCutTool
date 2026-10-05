# ShortCutTool

A Windows keyboard shortcut manager that uses **Meh** (Ctrl+Alt+Shift) and **Hyper** (Ctrl+Alt+Shift+Win) key combinations to launch and cycle through application windows.

## Features

✨ **Launch or cycle applications** with a single keyboard shortcut  
🔄 **Cycle through multiple windows** of the same application (e.g., multiple VS Code windows)  
⬅️ **Reverse cycling** using Hyper key combination  
📋 **Visual popup** shows window list while cycling (appears near system tray)  
🎯 **Stays visible** until you release the modifier keys  
⚡ **Lightweight** - runs in system tray with minimal resource usage  
🚀 **Auto-start** support via Windows Startup folder or Task Scheduler

## What are Meh and Hyper Keys?

- **Meh** = `Ctrl + Alt + Shift` (forward cycling)
- **Hyper** = `Ctrl + Alt + Shift + Win` (reverse cycling)

These combinations are rarely used by other applications, making them perfect for global shortcuts!

## Quick Start

### Installation

#### Option 1: Winget (Coming Soon)
```powershell
winget install Qisuk.ShortCutTool
```
*Note: Pending approval in Windows Package Manager repository*

#### Option 2: Installer
1. Download `ShortCutTool-<version>-win-x64-setup.exe` from the [latest release](https://github.com/Qisuk/ShortCutTool/releases/latest)
2. Run it. No admin rights are needed: it installs for the current user to `%LOCALAPPDATA%\Programs\ShortCutTool`, adds a Start Menu entry, and can start ShortCutTool when you sign in.

Upgrades close the running app, install, and start it again. Your shortcuts in `%APPDATA%\ShortCutTool` are kept.

#### Option 3: Portable ZIP
1. Download `ShortCutTool-<version>-win-x64.zip` from the [latest release](https://github.com/Qisuk/ShortCutTool/releases/latest)
2. Extract it anywhere and run `ShortCutTool.exe`

To verify a download, compare it with `SHA256SUMS.txt` from the same release:
```powershell
Get-FileHash .\ShortCutTool-*-win-x64* -Algorithm SHA256
```

#### Option 4: Build from Source
```powershell
dotnet test --solution ShortCutTool.slnx
.\installer\Build-Installer.ps1   # needs Inno Setup: winget install JRSoftware.InnoSetup
```
Artifacts (installer, ZIP, `SHA256SUMS.txt`) are written to `.\artifacts`.

### First Run

1. **Run** `ShortCutTool.exe` - it will appear in your system tray. On first run the Shortcut Manager opens so you can add your first shortcuts.

2. **Configure shortcuts**:
   - Right-click tray icon → "Show Shortcuts..."
   - Click "Add Shortcut" to add applications, then "Save & Restart"
   - Or edit `shortcuts.json` manually (right-click tray icon → "Open Config Folder")

3. **Use shortcuts**:
   - Press `Meh + Key` to launch or bring forward an application
   - If multiple windows exist, press again to cycle forward
   - Use `Hyper + Key` to cycle backward through windows

Only one copy of ShortCutTool runs at a time; starting it again shows a reminder to look in the system tray.

## Configuration

Shortcuts are stored per user in:

```
%APPDATA%\ShortCutTool\shortcuts.json
```

This location survives upgrades and reinstalls. If an older version left a `shortcuts.json` next to `ShortCutTool.exe`, it is copied here automatically the first time the new version starts. [`shortcuts.example.json`](shortcuts.example.json) in this repository has a fuller example.

Edit `shortcuts.json` to customize your shortcuts:

```json
{
  "shortcuts": [
    {
      "key": "C",
      "useMeh": true,
      "useHyperForReverse": true,
      "applicationPath": "C:\\Users\\%USERNAME%\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe",
      "workingDirectory": ""
    }
  ]
}
```

### Configuration Options

- **key**: The letter key to press (with Meh/Hyper)
- **useMeh**: Enable Meh (Ctrl+Alt+Shift) combination
- **useHyperForReverse**: Enable Hyper (Ctrl+Alt+Shift+Win) for reverse cycling
- **applicationPath**: Full path to the executable (supports environment variables like `%USERNAME%`)
- **workingDirectory**: Optional starting directory for the application

## Installation & Auto-Start

### Option 1: Tray Menu (Easiest)

Right-click the tray icon and tick **Start with Windows**. This adds a per-user entry under `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`; untick it to remove the entry.

### Option 2: Startup Folder

1. Build/publish the application
2. Create a shortcut to `ShortCutTool.exe`
3. Open Windows Startup folder:
   ```powershell
   explorer shell:startup
   ```
4. Paste the shortcut into the Startup folder

### Option 3: Task Scheduler (More Control)

1. Open Task Scheduler (`Win+R` → `taskschd.msc`)
2. Create Task (not Basic Task):
   - **General**: Name it "ShortCutTool", run only when logged on
   - **Triggers**: At log on for your user
   - **Actions**: Start program → browse to `ShortCutTool.exe`
   - **Conditions**: Uncheck "only on AC power"
   - **Settings**: Don't start new instance if already running

## Usage Examples

### Single Window Application
Press `Meh+N` → Notepad launches or comes to foreground

### Multiple Windows (e.g., VS Code)
1. Press `Meh+C` → First VS Code window comes forward
2. Press `Meh+C` again → Cycles to second VS Code window
3. Press `Meh+C` again → Cycles to third window (or wraps to first)
4. Press `Hyper+C` → Cycles backward through windows

### Visual Feedback
While holding the Meh/Hyper keys and cycling, a popup appears near your system tray showing:
- List of all windows
- Current selected window (highlighted)
- Popup disappears when you release the keys

## Troubleshooting

### Shortcuts not working
- Verify the app is running (check system tray)
- Ensure `%APPDATA%\ShortCutTool\shortcuts.json` is valid JSON
- Check the log: right-click tray icon → "Open Log Folder" (`%LOCALAPPDATA%\ShortCutTool\logs`)
- Check that application paths exist
- Try running as administrator if launching elevated apps

### Can't see tray icon
- Click the up arrow (^) in system tray
- Or: Taskbar settings → Other system tray icons → Enable ShortCutTool

### Application doesn't auto-start
- Check the shortcut exists in `shell:startup` folder
- Or verify the Task Scheduler task is enabled

### Cycling not working
- Ensure the application process is actually running multiple windows
- Check that `useHyperForReverse` is set to `true` in config

## Why Not a Windows Service?

Windows Services run in Session 0 (isolated from the user desktop) and cannot:
- Install keyboard hooks
- Interact with the desktop
- Launch GUI applications in the user session

This app runs as a **user-level tray application** to overcome these limitations.

## Uninstall

**Installed with the installer or WinGet:** use *Settings → Apps → Installed apps → ShortCut Tool → Uninstall*. This stops the app and removes the "Start with Windows" entry. Your shortcuts in `%APPDATA%\ShortCutTool` are kept; delete that folder too for a clean removal.

**Portable copy:**
1. Untick "Start with Windows" in the tray menu (or remove your Startup folder shortcut / Task Scheduler task)
2. Exit the application (right-click tray icon → Exit)
3. Delete the application files, and `%APPDATA%\ShortCutTool` if you want to remove your shortcuts

## System Requirements

- Windows 10 or later
- .NET 10 Runtime

## Releasing

1. Set `<Version>` in `ShortCutTool.csproj` (e.g. `1.1.0`) and add a section to `Documents/CHANGELOG.md`
2. Merge to `master`, then tag and push:
   ```powershell
   git tag v1.1.0
   git push origin v1.1.0
   ```
3. The **Release** workflow checks the tag matches the csproj version, runs the tests, builds the installer and ZIP with `installer\Build-Installer.ps1`, and publishes the GitHub release with `SHA256SUMS.txt`. Tags with a suffix (`v1.2.0-beta.1`) become pre-releases.

Publishing to WinGet (first submission and the automatic updates after it) is described in [Documents/WINGET.md](Documents/WINGET.md).

Every pull request also builds the installer; download it from the CI run's **ShortCutTool-installer** artifact to test before releasing.

## Contributing

Contributions welcome! Please open an issue or PR on GitHub.

## License

MIT License - see LICENSE file for details

---

**Tip**: Start with a few shortcuts, get comfortable with Meh key combinations, then add more as needed!





