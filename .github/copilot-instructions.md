# Copilot Instructions for ShortCutTool

ShortCutTool is a .NET 10 WinForms tray app (Windows only) that launches and cycles application windows with Meh (Ctrl+Alt+Shift) and Hyper (Ctrl+Alt+Shift+Win) shortcuts. Read `Documents/HOW-IT-WORKS.md` before changing the keyboard hook, window cycling or file locations.

## Layout

- `*.cs` in the repository root and `ShortCutTool/Program.cs` - the app (`ShortCutTool.csproj`)
- `tests/ShortCutTool.Tests` - xUnit v3 tests (`dotnet test --solution ShortCutTool.slnx`)
- `installer/` - Inno Setup script and `Build-Installer.ps1` (installer, portable ZIP, SHA256SUMS)
- `winget/` - WinGet manifest generator for the first submission
- `.github/workflows/` - `ci.yml` (PRs and master) and `release.yml` (version tags)
- `Documents/` - all Markdown docs except `README.md` (root) and this file

## Rules

- **Version**: change only `<Version>` in `ShortCutTool.csproj`. Releases are cut by pushing a matching `vX.Y.Z` tag; never add other version properties.
- **Config and logs** live in `%APPDATA%\ShortCutTool` and `%LOCALAPPDATA%\ShortCutTool\logs` (see `AppPaths`). Never read or write files relative to the working directory.
- **Logging**: use `Log.Info/Warn/Error`, not `Debug.WriteLine` or `Console`.
- **Shared names** that the installer and WinGet manifest depend on - keep them in sync or don't change them:
  - mutex `ShortCutTool.SingleInstance` (`Program.SingleInstanceMutexName`, Inno `MutexName`)
  - Run key value `ShortCutTool` (`StartupRegistration`, Inno `RunValueName`)
  - installer `AppGuid` / product code and `AppPublisher`
  - release asset names `ShortCutTool-<version>-win-x64-setup.exe` and `-win-x64.zip`
- **Docs**: put new Markdown in `Documents/`, and record user-visible changes in `Documents/CHANGELOG.md` under `[Unreleased]`.
- Add unit tests for logic that doesn't need a desktop session.
