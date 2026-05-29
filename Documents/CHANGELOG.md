# Changelog

All notable changes to ShortCutTool will be documented in this file.

## [1.0.0] - 2026

### Added
- **Meh key support** (Ctrl+Alt+Shift) for launching and activating applications
- **Hyper key support** (Ctrl+Alt+Shift+Win) for reverse cycling through windows
- **Multi-window cycling** - cycle through multiple instances of the same application
- **Visual popup UI** - shows list of windows and current selection while cycling
- **System tray integration** - runs in background with tray icon
- **JSON configuration** - easy-to-edit shortcuts.json file
- **Environment variable support** - use %USERNAME% and other variables in paths
- **Auto-start support** - via Windows Startup folder or Task Scheduler
- **Window enumeration** - finds all visible windows for an application
- **Focus activation** - brings windows to foreground reliably
- **Popup positioning** - appears near system tray/taskbar
- **Persistent display** - popup remains visible until modifier keys are released

### Features
- Launch applications if not running
- Bring single-window apps to foreground
- Cycle forward through multiple windows
- Cycle backward with Hyper key
- Update popup in real-time while cycling
- Non-intrusive, click-through popup
- Auto-dismiss when keys released
- Working directory support for launched apps

### Default Shortcuts
- Meh+N: Notepad
- Meh+X: Excel
- Meh+C: VS Code
- Meh+V: Visual Studio
- Meh+G: Chrome
- Meh+E: Edge
- Meh+Q: SQL Server Management Studio
- Meh+O: Outlook
- Meh+T: Teams

### Technical
- Built with .NET 10
- Windows Forms for UI
- Low-level keyboard hooks
- P/Invoke for Windows API integration
- Multi-threaded popup rendering
- Thread-safe window activation

### Documentation
- Comprehensive README
- Installation instructions
- Configuration examples
- Troubleshooting guide
- Contributing guidelines
- MIT License
