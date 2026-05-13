# 🚀 Winget Submission - Quick Reference

## One-Command Release
```powershell
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

## Validate Before Submitting
```powershell
.\Validate-WingetManifests.ps1
```

## Full Process (30 minutes)

### 1️⃣ Create Release Package (5 min)
```powershell
.\Create-WingetRelease.ps1 -Version "1.0.0"
```
✅ Builds app, creates ZIP, calculates hash, updates manifests

### 2️⃣ GitHub Release (5 min)
1. Go to: https://github.com/Qisuk/ShortCutTool/releases/new
2. Tag: `v1.0.0`
3. Upload: `release\ShortCutTool-v1.0.0.zip`
4. Click "Publish release"

### 3️⃣ Fork & Clone winget-pkgs (5 min)
```powershell
# On GitHub: Fork https://github.com/microsoft/winget-pkgs
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
cd winget-pkgs
```

### 4️⃣ Copy Manifests (2 min)
```powershell
mkdir manifests\q\Qisuk\ShortCutTool\1.0.0
Copy-Item ..\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.0.0\
```

### 5️⃣ Validate & Submit (5 min)
```powershell
winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\
git checkout -b qisuk-shortcuttool-1.0.0
git add manifests\q\Qisuk\ShortCutTool\1.0.0\
git commit -m "Add Qisuk.ShortCutTool version 1.0.0"
git push origin qisuk-shortcuttool-1.0.0
```

### 6️⃣ Create PR (3 min)
1. Go to: https://github.com/microsoft/winget-pkgs
2. Click "New Pull Request"
3. Select your fork/branch
4. Title: `Add Qisuk.ShortCutTool version 1.0.0`
5. Submit

### 7️⃣ Wait for Approval (2-7 days)
- Automated validation runs
- Maintainers review
- Merge & published

## After Approval
Users install with:
```powershell
winget install Qisuk.ShortCutTool
```

## Files Created
```
.winget/
├── Qisuk.ShortCutTool.yaml (version info)
├── Qisuk.ShortCutTool.installer.yaml (installer + SHA256)
└── Qisuk.ShortCutTool.locale.en-US.yaml (metadata)

Create-WingetRelease.ps1 (automation script)
Validate-WingetManifests.ps1 (validation script)
WINGET_SUBMISSION.md (detailed guide)
WINGET_HOWTO.md (overview)
```

## Troubleshooting

❌ **SHA256 placeholder error**
```powershell
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

❌ **Validation fails**
```powershell
.\Validate-WingetManifests.ps1
```

❌ **Wrong package identifier**
- Can't change after first submission!
- Currently: `Qisuk.ShortCutTool`

## Resources
📚 [Full Guide](WINGET_SUBMISSION.md)
📖 [Overview](WINGET_HOWTO.md)
🔗 [Winget Docs](https://learn.microsoft.com/windows/package-manager/)

---
**Ready to start?** Run: `.\Create-WingetRelease.ps1 -Version "1.0.0"`
