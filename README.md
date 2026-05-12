# ShortCutTool - Auto-Start Application

This application runs as a background application in your system tray and will start automatically when you log in to Windows.

## Why Not a Windows Service?

Windows Services run in Session 0 (isolated from user desktop) and cannot install keyboard hooks or interact with the desktop. Therefore, this application runs as a **user-level auto-start application** instead.

## Installation & Auto-Start Setup

### Option 1: Using Windows Startup Folder (Recommended)

1. Build and publish the application:
   ```powershell
   dotnet publish -c Release -o ./publish
   ```

2. Create a shortcut to the executable:
   - Right-click on `ShortCutTool.exe` in the publish folder
   - Select "Create shortcut"

3. Copy the shortcut to your Startup folder:
   ```powershell
   # Open the Startup folder
   explorer shell:startup
   ```
   Then paste the shortcut into this folder

4. The application will now start automatically when you log in

### Option 2: Using Task Scheduler (More Control)

1. Open Task Scheduler (Win+R, type `taskschd.msc`)

2. Click "Create Task" (not "Create Basic Task")

3. **General Tab**:
   - Name: ShortCutTool
   - Description: Keyboard shortcut manager
   - Run whether user is logged on or not: **NO** (must run only when logged on)
   - Run with highest privileges: Check this if you need to launch apps with admin rights

4. **Triggers Tab**:
   - New Trigger
   - Begin the task: "At log on"
   - Specific user: Your username
   - Click OK

5. **Actions Tab**:
   - New Action
   - Action: Start a program
   - Program/script: Browse to `ShortCutTool.exe`
   - Start in: The folder containing the executable
   - Click OK

6. **Conditions Tab**:
   - Uncheck "Start the task only if the computer is on AC power"

7. **Settings Tab**:
   - Check "Allow task to be run on demand"
   - If the task is already running: "Do not start a new instance"

8. Click OK to save

## Running the Application

### Start Manually
Just double-click `ShortCutTool.exe`. It will run in the system tray.

### Stop the Application
- Right-click the system tray icon and select "Exit"
- Or use Task Manager to end the process

## Configuration

The application reads `shortcuts.json` from the same directory as the executable.

To modify shortcuts:
1. Exit the application (right-click tray icon → Exit)
2. Edit `shortcuts.json`
3. Start the application again

## System Tray Icon

The application runs in the system tray (notification area):
- **Double-click** the icon to see status information
- **Right-click** the icon for options (Exit)
- The tooltip shows how many shortcuts are active

## Troubleshooting

### Application doesn't start automatically
- **Startup Folder Method**: Check that the shortcut is in `shell:startup`
- **Task Scheduler Method**: Open Task Scheduler and verify the task exists and is enabled
- Make sure the path in the shortcut/task points to the correct location

### Access Denied or Permission Issues
- The application must run in your user session (not as a service)
- If launching admin apps, run the ShortCutTool with "Run as administrator"
- Or configure the Task Scheduler task to "Run with highest privileges"

### Shortcuts not working
- Make sure the application is running (check system tray)
- Verify `shortcuts.json` is valid JSON
- Check that application paths in the config are correct

### Can't see tray icon
- Click the up arrow (^) in the system tray to show hidden icons
- Right-click taskbar → Taskbar settings → Other system tray icons → Enable ShortCutTool

## Uninstall

1. **Remove from Startup Folder**:
   ```powershell
   explorer shell:startup
   ```
   Delete the ShortCutTool shortcut

2. **Or Remove from Task Scheduler**:
   - Open Task Scheduler
   - Find "ShortCutTool" task
   - Right-click → Delete

3. Delete the application files

## Example Configuration

```json
{
  "Shortcuts": [
    {
      "Key": "C",
      "UseMeh": true,
      "ApplicationPath": "C:\\Users\\YourUsername\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"
    },
    {
      "Key": "V",
      "UseMeh": true,
      "ApplicationPath": "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe"
    }
  ]
}
```

Press **Ctrl+Alt+Shift+C** to launch or switch to VS Code  
Press **Ctrl+Alt+Shift+V** to launch or switch to Visual Studio

