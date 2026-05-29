# ⚠️ Important: Winget-pkgs Repository Location

## Issue

If you clone `microsoft/winget-pkgs` inside your ShortCutTool project folder (e.g., in `release\winget-pkgs\`), MSBuild will try to compile its C# files and fail with errors like:

```
error CS0234: The type or namespace name 'Management' does not exist in the namespace 'System'
```

## Solution

**Clone `winget-pkgs` OUTSIDE your ShortCutTool project folder.**

### ✅ Correct Structure

```
C:\Users\chris\source\repos\
│
├── ShortCutTool\
│   └── ShortCutTool\          ← Your project
│       ├── ShortCutTool.csproj
│       ├── Program.cs
│       └── release\           ← Build artifacts only
│
└── winget-pkgs\               ← Clone HERE (separate folder)
	└── manifests\
```

### ❌ Wrong Structure

```
C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\
├── ShortCutTool.csproj
├── Program.cs
└── release\
	└── winget-pkgs\           ← Don't clone here!
		└── manifests\
```

## Commands for Manual Submission

If you need to manually submit to winget, clone the repository at the correct location:

```powershell
# Navigate to the parent repos folder
cd C:\Users\chris\source\repos\

# Clone winget-pkgs here (NOT inside ShortCutTool)
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git

# Now you have both repos side-by-side
# ShortCutTool project at: C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\
# winget-pkgs at: C:\Users\chris\source\repos\winget-pkgs\
```

## For Automated Publishing

If you're using GitHub Actions (recommended), you don't need to clone `winget-pkgs` at all!

See: [GITHUB_ACTIONS_WINGET.md](GITHUB_ACTIONS_WINGET.md)

The automation handles everything without requiring a local `winget-pkgs` clone.

## Protection

The `.gitignore` file has been updated to prevent accidentally committing `winget-pkgs/` directories:

```gitignore
# Winget package repository (should be cloned outside project)
winget-pkgs/
```

## Quick Fix

If you accidentally clone `winget-pkgs` inside your project and get build errors:

```powershell
# Remove the incorrectly placed repository
Remove-Item release\winget-pkgs -Recurse -Force

# Or wherever it was cloned:
Remove-Item winget-pkgs -Recurse -Force

# Rebuild
dotnet build
```

## Summary

✅ **Do:** Clone `winget-pkgs` as a sibling to your ShortCutTool folder
❌ **Don't:** Clone `winget-pkgs` inside your ShortCutTool project

✅ **Better:** Use GitHub Actions automation (no manual clone needed)

See the [winget submission documentation](WINGET_SUBMISSION_COMPLETE.md) for proper folder structure.
