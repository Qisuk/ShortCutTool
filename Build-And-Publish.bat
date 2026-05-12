@echo off
echo Building ShortCutTool...
echo ========================
echo.

if exist publish (
    echo Cleaning previous build...
    rmdir /s /q publish
)

echo Publishing application...
dotnet publish -c Release -o publish --self-contained false

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCESS] Build successful!
    echo.
    echo Published to: %cd%\publish
    echo.
    echo Next steps:
    echo 1. Copy the 'publish' folder to your desired location
    echo 2. Edit 'shortcuts.json' with your application paths
    echo 3. Run Setup-AutoStart.bat to configure auto-start
    echo.

    set /p response="Open publish folder now? (Y/N): "
    if /i "%response%"=="Y" (
        explorer publish
    )
) else (
    echo.
    echo [ERROR] Build failed. Please check the errors above.
    exit /b 1
)
