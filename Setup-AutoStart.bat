@echo off
setlocal

if "%~1"=="" (
    set "ExePath=.\bin\Release\net10.0-windows\ShortCutTool.exe"
) else (
    set "ExePath=%~1"
)

echo ShortCutTool Auto-Start Setup
echo =============================
echo.

if not exist "%ExePath%" (
    echo [ERROR] Executable not found at: %ExePath%
    echo Please build the application first or specify the correct path
    echo.
    echo Example: Setup-AutoStart.bat "C:\Path\To\ShortCutTool.exe"
    exit /b 1
)

for %%i in ("%ExePath%") do set "ExeFullPath=%%~fi"
echo Found executable: %ExeFullPath%
echo.

echo Creating shortcut in Startup folder...

set "StartupFolder=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"
set "ShortcutPath=%StartupFolder%\ShortCutTool.lnk"

powershell -ExecutionPolicy Bypass -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%ShortcutPath%'); $Shortcut.TargetPath = '%ExeFullPath%'; $Shortcut.WorkingDirectory = '%~dp0'; $Shortcut.Description = 'ShortCutTool - Keyboard Shortcut Manager'; $Shortcut.Save()"

if %ERRORLEVEL% EQU 0 (
    echo [SUCCESS] Shortcut created successfully!
    echo   Location: %ShortcutPath%
    echo.

    set /p response="Do you want to start ShortCutTool now? (Y/N): "
    if /i "!response!"=="Y" (
        echo Starting ShortCutTool...
        start "" "%ExeFullPath%"
        echo [SUCCESS] ShortCutTool started! Check your system tray.
    )

    echo.
    echo Setup complete! ShortCutTool will now start automatically when you log in.
    echo.
    echo To remove from startup, delete: %ShortcutPath%
) else (
    echo [ERROR] Failed to create shortcut.
    exit /b 1
)

endlocal
