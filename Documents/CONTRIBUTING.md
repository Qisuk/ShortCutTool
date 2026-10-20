# Contributing to ShortCutTool

Thank you for your interest in contributing! Here's how you can help:

## Reporting Issues

When reporting bugs, please include:
- Windows version
- ShortCutTool version (tray menu → About)
- Steps to reproduce
- Expected vs actual behavior
- The log file: tray menu → Open Log Folder (`%LOCALAPPDATA%\ShortCutTool\logs`)

## Feature Requests

Feel free to open an issue to suggest new features. Please describe:
- The use case
- Expected behavior
- Why it would be useful

## Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Test thoroughly
5. Commit with clear messages (`git commit -m 'Add amazing feature'`)
6. Push to your fork (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## Development Setup

1. Install the .NET 10 SDK (the exact version is pinned in `global.json`)
2. Clone the repository and open `ShortCutTool.slnx` in Visual Studio 2022 17.14+ / 2026, or VS Code with C# Dev Kit
3. Build and test:
   ```powershell
   dotnet build ShortCutTool.slnx
   dotnet test --solution ShortCutTool.slnx
   ```
4. Run: `dotnet run --project ShortCutTool.csproj`. Exit any installed copy first; only one instance runs at a time.
5. To build the installer: install Inno Setup (`winget install JRSoftware.InnoSetup`) and run `.\installer\Build-Installer.ps1`

[HOW-IT-WORKS.md](HOW-IT-WORKS.md) explains the keyboard hook, window cycling and where files live. CI builds, tests and produces the installer for every pull request.

## Code Style

- Follow existing code conventions
- Use meaningful variable names
- Add comments for complex logic
- Keep methods focused and small

## Testing

- Add or update unit tests in `tests/ShortCutTool.Tests` for logic that doesn't need a desktop (such as `ConfigValidator`)
- Test with multiple windows of the same application
- Test with applications that don't exist
- Test the popup UI on different screen configurations
- Test "Start with Windows", and install / upgrade / uninstall with the installer from the PR's CI run

## Areas for Contribution

- Additional keyboard hook improvements
- UI/UX enhancements for the popup
- Better error handling
- Performance optimizations
- Documentation improvements

Thank you for contributing!
