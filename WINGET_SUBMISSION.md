# ShortCutTool Winget Submission Guide

## Prerequisites
1. Install wingetcreate: `winget install wingetcreate`
2. Fork https://github.com/microsoft/winget-pkgs
3. Clone your fork locally

## Steps to Submit

### 1. Create GitHub Release (if not done already)
```powershell
# Build release version
dotnet publish -c Release -o .\publish --self-contained false -r win-x64

# Create a zip file
Compress-Archive -Path .\publish\* -DestinationPath ShortCutTool-v1.0.0.zip

# Upload to GitHub Releases at:
# https://github.com/Qisuk/ShortCutTool/releases/new
```

### 2. Get SHA256 Hash
```powershell
certutil -hashfile ShortCutTool-v1.0.0.zip SHA256
```
Copy the hash and update `.winget/Qisuk.ShortCutTool.installer.yaml`

### 3. Clone Winget Packages Repository
```powershell
# Fork https://github.com/microsoft/winget-pkgs first
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
cd winget-pkgs
```

### 4. Create Package Folder
```powershell
# Create folder structure
mkdir manifests\q\Qisuk\ShortCutTool\1.0.0
```

### 5. Copy Manifest Files
```powershell
# Copy your manifest files from .winget folder to the new location
Copy-Item ..\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.0.0\
```

### 6. Validate Manifests
```powershell
# Test the manifests locally
winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\
```

### 7. Create Pull Request
```powershell
# Create a new branch
git checkout -b qisuk-shortcuttool-1.0.0

# Add files
git add manifests/q/Qisuk/ShortCutTool/1.0.0/

# Commit
git commit -m "Add Qisuk.ShortCutTool version 1.0.0"

# Push to your fork
git push origin qisuk-shortcuttool-1.0.0
```

### 8. Submit Pull Request
1. Go to https://github.com/microsoft/winget-pkgs
2. Click "New Pull Request"
3. Select your fork and branch
4. Title: `Add Qisuk.ShortCutTool version 1.0.0`
5. Description: Brief description of your app
6. Submit the PR

### 9. Wait for Review
- Automated checks will validate your manifest
- Maintainers will review your submission
- Once approved and merged, your app will be available via winget

## Testing Before Submission
```powershell
# Test local install (from your manifest folder)
winget install --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\
```

## After Approval
Users can install your app with:
```powershell
winget install Qisuk.ShortCutTool
```

## Updating Later
For new versions (e.g., 1.1.0):
```powershell
# Use wingetcreate to update
wingetcreate update Qisuk.ShortCutTool -v 1.1.0 -u https://github.com/Qisuk/ShortCutTool/releases/download/v1.1.0/ShortCutTool-v1.1.0.zip
```

## Resources
- Winget Manifest Schema: https://aka.ms/winget-manifest
- Submission Guidelines: https://github.com/microsoft/winget-pkgs/blob/master/CONTRIBUTING.md
- Winget CLI Docs: https://learn.microsoft.com/windows/package-manager/
