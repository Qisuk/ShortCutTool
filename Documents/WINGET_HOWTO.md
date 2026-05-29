# Making ShortCutTool Available via Winget - Summary

## ✅ Files Created

I've created several files to help you submit ShortCutTool to winget:

### 1. **Manifest Files** (`.winget/` folder)
- `Qisuk.ShortCutTool.yaml` - Version manifest
- `Qisuk.ShortCutTool.installer.yaml` - Installer details with SHA256 hash placeholder
- `Qisuk.ShortCutTool.locale.en-US.yaml` - Localization and metadata

### 2. **Helper Scripts**
- `Create-WingetRelease.ps1` - Automated release package creation
- `WINGET_SUBMISSION.md` - Complete step-by-step submission guide

### 3. **Updated Documentation**
- `README.md` - Added winget installation instructions

## 🚀 Quick Start to Submit

### Step 1: Create a Release Package
```powershell
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

This will:
- Build the release
- Create a ZIP file
- Calculate SHA256 hash
- Update manifest files automatically

### Step 2: Create GitHub Release
1. Go to https://github.com/Qisuk/ShortCutTool/releases/new
2. Create tag `v1.0.0`
3. Upload the generated ZIP file from `.\release\ShortCutTool-v1.0.0.zip`
4. Publish the release

### Step 3: Submit to Winget
Follow the detailed instructions in `WINGET_SUBMISSION.md`:
1. Fork `microsoft/winget-pkgs` repository
2. Clone your fork
3. Copy manifest files to correct location
4. Validate manifests
5. Create pull request

## 📋 Before Submitting Checklist

- [ ] Application builds successfully
- [ ] GitHub repository is public
- [ ] LICENSE file exists (✅ MIT License already added)
- [ ] README is complete (✅ Already updated)
- [ ] Create GitHub Release with binaries
- [ ] Test manifest files locally: `winget validate --manifest .winget\`
- [ ] Manifest files have correct SHA256 hash

## ⏱️ Timeline

1. **Immediate**: Create release package and GitHub Release (10 minutes)
2. **Day 1-2**: Submit pull request to winget-pkgs
3. **Day 2-7**: Wait for automated checks and review
4. **After merge**: App becomes available via winget within hours

## 🎯 After Approval

Users can install with:
```powershell
winget install Qisuk.ShortCutTool
```

Or search for it:
```powershell
winget search ShortCutTool
```

## 📦 Alternative Distribution Methods

While waiting for winget approval, you can also distribute via:

1. **Chocolatey** - Another package manager for Windows
2. **Scoop** - Lightweight package manager
3. **Direct GitHub Releases** - Already set up
4. **Microsoft Store** - Requires app package and submission

## 🔄 Updating Later

For version updates (e.g., 1.1.0):
```powershell
.\Create-WingetRelease.ps1 -Version "1.1.0"

# Then use wingetcreate to submit update
wingetcreate update Qisuk.ShortCutTool -v 1.1.0 -u https://github.com/Qisuk/ShortCutTool/releases/download/v1.1.0/ShortCutTool-v1.1.0.zip
```

## 📚 Resources

- **Winget Documentation**: https://learn.microsoft.com/windows/package-manager/
- **Winget Packages Repo**: https://github.com/microsoft/winget-pkgs
- **Contribution Guide**: https://github.com/microsoft/winget-pkgs/blob/master/CONTRIBUTING.md
- **Manifest Schema**: https://aka.ms/winget-manifest

## ⚠️ Important Notes

1. **Package Identifier**: `Qisuk.ShortCutTool` - This becomes permanent, choose carefully
2. **Installer Type**: Currently set to `zip` - you may want to create a proper installer (MSI/EXE) later
3. **Self-contained**: Set to `false` - requires .NET 10 runtime. Consider `true` for easier installation
4. **First submission**: May take longer (7-10 days) due to extra scrutiny

## 💡 Recommendations

1. **Create a proper installer** using WiX or Inno Setup for better user experience
2. **Add screenshots** to GitHub README to showcase the app
3. **Create a demo GIF** showing the app in action
4. **Set up GitHub Actions** for automated releases
5. **Consider code signing** for the executable (builds trust)

## Next Steps

Run this to get started:
```powershell
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

Then follow `WINGET_SUBMISSION.md` for detailed submission steps!
