# ShortCutTool - Changes Summary

## What Changed

Your application has been converted from a **console application** to a **Windows background application** that:
- Runs silently in the system tray (no console window)
- Can be configured to start automatically with Windows
- Works correctly with keyboard hooks (which don't work in Windows Services)

## Why Not a Windows Service?

Windows Services run in "Session 0" which is isolated from the user desktop. This prevents:
- Installing global keyboard hooks (security restriction)
- Interacting with desktop applications
- Launching GUI applications in the user's session

The solution is to run as a **user-level application** that starts automatically when you log in.

## Key Changes Made

### 1. Project File (ShortCutTool.csproj)
- Changed target framework to `net10.0-windows` (required for Windows Forms)
- Set `OutputType` to `WinExe` (no console window)
- Enabled `UseWindowsForms` for system tray support
- Removed Windows Service packages (not needed)

### 2. Program.cs
- Converted from console app to Windows Forms application
- Uses MessageBox for user notifications instead of Console
- Initializes system tray application
- Simplified startup logic

### 3. New Files

**TrayApplicationContext.cs**
- Manages the system tray icon
- Provides right-click context menu
- Shows application status
- Handles exit gracefully

**README.md**
- Complete installation instructions
- Two methods for auto-start setup:
  - Startup folder (simple)
  - Task Scheduler (advanced)
- Troubleshooting guide

**Setup-AutoStart.ps1**
- PowerShell script for easy auto-start configuration
- Creates shortcut in Windows Startup folder
- Can optionally start the application immediately

**Build-And-Publish.ps1**
- Builds the application in Release mode
- Creates a publish folder ready for deployment
- Interactive prompts for next steps

### 4. Removed Files
- Worker.cs (no longer needed - was for Windows Service)

## How to Use

### Quick Start
1. Build and publish:
   ```powershell
   .\Build-And-Publish.ps1
   ```

2. Set up auto-start:
   ```powershell
   .\Setup-AutoStart.ps1 -ExePath ".\publish\ShortCutTool.exe"
   ```

3. Edit `publish\shortcuts.json` with your application paths

4. Start the application (it will also start automatically on next login)

### Manual Start
Just double-click `ShortCutTool.exe` - it will appear in the system tray.

### Stop
Right-click the system tray icon and select "Exit"

## Technical Details

### Application Flow
1. Application starts (hidden window, no console)
2. Checks for `shortcuts.json` config file
3. Creates default config if missing
4. Initializes keyboard hook service
5. Registers all shortcuts from config
6. Creates system tray icon
7. Runs message pump (keeps app alive)
8. On exit: unhooks keyboard, disposes tray icon

### Keyboard Hook
- Still uses low-level keyboard hook (unchanged)
- Works properly in user session (not Session 0)
- Listens for Meh (Ctrl+Alt+Shift) combinations
- Launches or switches to configured applications

### System Tray
- Shows application is running
- Tooltip displays number of active shortcuts
- Double-click shows status message
- Right-click provides Exit option

## Troubleshooting

### Common Issues

**"Access Denied" when running**
- **Solution**: This error should no longer occur since we're not using Windows Service
- If you still see it, make sure you're not trying to install as a service

**Keyboard hooks not working**
- **Cause**: Application must run in user session
- **Solution**: Don't install as a Windows Service; use auto-start methods in README.md

**Can't see tray icon**
- **Cause**: Icon might be in hidden tray area
- **Solution**: Click the up arrow (^) in system tray to show hidden icons

**Application doesn't start automatically**
- **Check**: Verify shortcut exists in Startup folder (`shell:startup`)
- **Or**: Check Task Scheduler has the task configured correctly

## Benefits of This Approach

✅ Keyboard hooks work correctly (user session)  
✅ No "Access Denied" errors  
✅ Can launch and switch to GUI applications  
✅ Runs silently in background  
✅ Auto-starts with Windows login  
✅ Easy to manage (tray icon)  
✅ No administrator privileges needed (unless launching admin apps)  

## Migration from Old Setup

If you previously tried to install as a Windows Service:

1. Remove the service:
   ```powershell
   sc.exe stop ShortCutTool
   sc.exe delete ShortCutTool
   ```

2. Follow the new setup instructions in README.md

## Additional Notes

- The application targets .NET 10.0 as specified
- All original functionality (keyboard hooks, app launching, cycling) remains unchanged
- Only the hosting and startup mechanism changed
- Configuration file format is identical
