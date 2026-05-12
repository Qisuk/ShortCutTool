# Build and Publish ShortCutTool
# This script builds the application in Release mode and creates a ready-to-deploy folder

Write-Host "Building ShortCutTool..." -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
Write-Host ""

# Clean previous builds
if (Test-Path ".\publish") {
    Write-Host "Cleaning previous build..." -ForegroundColor Yellow
    Remove-Item -Path ".\publish" -Recurse -Force
}

# Build and publish
Write-Host "Publishing application..." -ForegroundColor Yellow
dotnet publish -c Release -o .\publish --self-contained false

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "[SUCCESS] Build successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Published to: $(Resolve-Path .\publish)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Copy the 'publish' folder to your desired location" -ForegroundColor White
    Write-Host "2. Edit 'shortcuts.json' with your application paths" -ForegroundColor White
    Write-Host "3. Run Setup-AutoStart.ps1 to configure auto-start" -ForegroundColor White
    Write-Host "   Example: .\Setup-AutoStart.ps1 -ExePath '.\publish\ShortCutTool.exe'" -ForegroundColor Gray
    Write-Host ""

    # Ask if user wants to open publish folder
    $response = Read-Host "Open publish folder now? (Y/N)"
    if ($response -eq 'Y' -or $response -eq 'y') {
        explorer .\publish
    }
} else {
    Write-Host ""
    Write-Host "[ERROR] Build failed. Please check the errors above." -ForegroundColor Red
    exit 1
}
