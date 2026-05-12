# ShortCutTool Auto-Start Setup Script
# Run this script after building/publishing your application

param(
    [Parameter(Mandatory=$false)]
    [string]$ExePath = ".\bin\Release\net10.0-windows\ShortCutTool.exe"
)

Write-Host "ShortCutTool Auto-Start Setup" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

# Check if executable exists
if (-not (Test-Path $ExePath)) {
    Write-Host "Error: Executable not found at: $ExePath" -ForegroundColor Red
    Write-Host "Please build the application first or specify the correct path using -ExePath parameter" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Example: .\Setup-AutoStart.ps1 -ExePath 'C:\Path\To\ShortCutTool.exe'" -ForegroundColor Yellow
    exit 1
}

$ExeFullPath = Resolve-Path $ExePath
Write-Host "Found executable: $ExeFullPath" -ForegroundColor Green
Write-Host ""

# Create shortcut
$WshShell = New-Object -ComObject WScript.Shell
$StartupFolder = $WshShell.SpecialFolders("Startup")
$ShortcutPath = Join-Path $StartupFolder "ShortCutTool.lnk"

Write-Host "Creating shortcut in Startup folder..." -ForegroundColor Yellow
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = $ExeFullPath
$Shortcut.WorkingDirectory = Split-Path $ExeFullPath
$Shortcut.Description = "ShortCutTool - Keyboard Shortcut Manager"
$Shortcut.Save()

Write-Host "[SUCCESS] Shortcut created successfully!" -ForegroundColor Green
Write-Host "  Location: $ShortcutPath" -ForegroundColor Gray
Write-Host ""

# Ask if user wants to start now
$response = Read-Host "Do you want to start ShortCutTool now? (Y/N)"
if ($response -eq 'Y' -or $response -eq 'y') {
    Write-Host "Starting ShortCutTool..." -ForegroundColor Yellow
    Start-Process $ExeFullPath
    Write-Host "[SUCCESS] ShortCutTool started! Check your system tray." -ForegroundColor Green
}

Write-Host ""
Write-Host "Setup complete! ShortCutTool will now start automatically when you log in." -ForegroundColor Green
Write-Host ""
Write-Host "To remove from startup, delete the shortcut from: $StartupFolder" -ForegroundColor Gray
