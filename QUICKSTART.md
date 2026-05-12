# 🚀 Quick Start Guide

Get ShortCutTool up and running in 3 minutes!

## Step 1: Build the Application

**Option A: Using Batch File (Easiest - No PowerShell Issues)**
```cmd
Build-And-Publish.bat
```

**Option B: Using PowerShell Script**
```powershell
.\Build-And-Publish.ps1
```

If you get a PowerShell execution policy error, see [POWERSHELL-HELP.md](POWERSHELL-HELP.md)

This creates a `publish` folder with everything you need.

## Step 2: Configure Your Shortcuts

1. Open `publish\shortcuts.json` in a text editor
2. Update the application paths to match your system
3. Add or remove shortcuts as needed

Example:
```json
{
  "Shortcuts": [
    {
      "Key": "C",
      "UseMeh": true,
      "ApplicationPath": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
    },
    {
      "Key": "V",
      "UseMeh": true,
      "ApplicationPath": "C:\\Users\\YourName\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"
    }
  ]
}
```

## Step 3: Set Up Auto-Start

**Option A: Using Batch File (Easiest - No PowerShell Issues)**
```cmd
Setup-AutoStart.bat publish\ShortCutTool.exe
```

**Option B: Using PowerShell Script**
```powershell
.\Setup-AutoStart.ps1 -ExePath ".\publish\ShortCutTool.exe"
```

When prompted, press `Y` to start the application now.

## ✅ Done!

You should now see a small icon in your system tray. 

- **Double-click** the tray icon to see status
- **Right-click** the tray icon for options
- The application will now start automatically when you log in to Windows

## Testing Your Shortcuts

Try pressing **Ctrl+Alt+Shift+C** (or whatever key you configured) to launch or switch to your application!

## PowerShell Script Issues?

If you get "cannot be loaded because running scripts is disabled", you have 3 options:

1. **Use the batch files instead** (`.bat` files) - They always work!
2. See [POWERSHELL-HELP.md](POWERSHELL-HELP.md) for solutions
3. Use manual steps in [README.md](README.md)

## Need Help?

- **Full documentation**: See [README.md](README.md)
- **PowerShell issues**: See [POWERSHELL-HELP.md](POWERSHELL-HELP.md)
- **What changed**: See [CHANGES.md](CHANGES.md)
- **Troubleshooting**: Check README.md "Troubleshooting" section

---

### Common Shortcuts Legend

- **Meh** = Ctrl + Alt + Shift pressed together
- **Key** = The letter or number to complete the shortcut

Example: If you set Key="C" and UseMeh=true, press **Ctrl+Alt+Shift+C**
