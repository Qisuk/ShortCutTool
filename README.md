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

1. **Download** the latest release or build from source:
   ```powershell
   dotnet publish -c Release -o ./publish
   ```

2. **Configure** your shortcuts by editing `shortcuts.json` (see examples below)

3. **Run** `ShortCutTool.exe` - it will appear in your system tray

4. **Use shortcuts**:
   - Press `Meh + Key` to launch or bring forward an application
   - If multiple windows exist, press again to cycle forward
   - Use `Hyper + Key` to cycle backward through windows

## Default Configuration

The included `shortcuts.json` has these defaults (customize to your needs):

| Key | Application | Shortcut |
|-----|-------------|----------|
| N | Notepad | Meh+N |
| X | Excel | Meh+X |
| C | VS Code | Meh+C |
| V | Visual Studio | Meh+V |
| G | Chrome | Meh+G |
| E | Edge | Meh+E |
| Q | SQL Server Management Studio | Meh+Q |
| O | Outlook | Meh+O |
| T | Teams | Meh+T |

## Configuration

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

### Option 1: Startup Folder (Easiest)

1. Build/publish the application
2. Create a shortcut to `ShortCutTool.exe`
3. Open Windows Startup folder:
   ```powershell
   explorer shell:startup
   ```
4. Paste the shortcut into the Startup folder

### Option 2: Task Scheduler (More Control)

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
- Ensure `shortcuts.json` is valid JSON
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

1. Exit the application (right-click tray icon → Exit)
2. Remove from Startup folder or delete Task Scheduler task
3. Delete the application files

## System Requirements

- Windows 10 or later
- .NET 10 Runtime

## Contributing

Contributions welcome! Please open an issue or PR on GitHub.

## License

MIT License - see LICENSE file for details

---

**Tip**: Start with a few shortcuts, get comfortable with Meh key combinations, then add more as needed!

