# 📦 Winget Submission Guide - Complete Step-by-Step

## 🎯 Overview

This guide shows you **exactly** how to submit ShortCutTool to the Windows Package Manager (winget) repository, including the complete folder structure at every step.

**Time Required:** 30-45 minutes (first time), 15 minutes (subsequent versions)

---

## 📋 Prerequisites

Before starting, ensure you have:

- ✅ GitHub account
- ✅ Git installed locally
- ✅ Winget CLI installed (`winget --version` works)
- ✅ PowerShell 5.1 or higher

---

## 🚀 Step-by-Step Submission Process

### Step 1: Prepare Your Release (5 minutes)

#### 1.1 Run the Release Script

From your ShortCutTool project directory:

```powershell
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

#### 1.2 Verify Files Created

After running the script, your folder structure should look like this:

```
C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\
│
├── .winget\                                      ← Manifest files (updated)
│   ├── Qisuk.ShortCutTool.yaml
│   ├── Qisuk.ShortCutTool.installer.yaml        ← SHA256 hash updated!
│   └── Qisuk.ShortCutTool.locale.en-US.yaml
│
├── release\                                      ← Release artifacts
│   ├── ShortCutTool-v1.0.0.zip                  ← Upload this to GitHub!
│   ├── ShortCutTool-v1.0.0.sha256               ← Hash for reference
│   └── publish\                                  ← Build output
│       ├── ShortCutTool.exe
│       ├── shortcuts.json
│       └── [other DLLs and runtime files]
│
├── ShortCutTool.csproj
├── Program.cs
├── README.md
└── [other project files]
```

**✅ Checkpoint:** You should have `release\ShortCutTool-v1.0.0.zip` ready to upload!

---

### Step 2: Create GitHub Release (5 minutes)

#### 2.1 Upload Release to GitHub

1. **Open your browser** and go to:
   ```
   https://github.com/Qisuk/ShortCutTool/releases/new
   ```

2. **Fill in the release form:**
   - **Tag version:** `v1.0.0` (must start with `v`)
   - **Release title:** `ShortCutTool v1.0.0`
   - **Description:** Copy from `.winget\Qisuk.ShortCutTool.locale.en-US.yaml` (ReleaseNotes section)

3. **Upload the ZIP file:**
   - Click "Attach binaries"
   - Select `release\ShortCutTool-v1.0.0.zip`
   - Wait for upload to complete

4. **Publish:**
   - Click "Publish release"

5. **Copy the download URL:**
   After publishing, right-click the ZIP file link and copy the URL. It should look like:
   ```
   https://github.com/Qisuk/ShortCutTool/releases/download/v1.0.0/ShortCutTool-v1.0.0.zip
   ```

**✅ Checkpoint:** Your release is now public at `https://github.com/Qisuk/ShortCutTool/releases`

---

### Step 3: Fork winget-pkgs Repository (One-Time Setup - 3 minutes)

#### 3.1 Fork the Repository

1. **Go to:** https://github.com/microsoft/winget-pkgs
2. **Click** the "Fork" button (top-right corner)
3. **Create fork** in your account

You now have: `https://github.com/YOUR_USERNAME/winget-pkgs`

#### 3.2 Clone Your Fork

```powershell
# Navigate to a working directory (NOT inside ShortCutTool)
cd C:\Users\chris\source\repos\

# Clone your fork
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
cd winget-pkgs
```

**Your folder structure is now:**

```
C:\Users\chris\source\repos\
│
├── ShortCutTool\                    ← Your app
│   └── ShortCutTool\
│       ├── .winget\                 ← Source manifests
│       └── release\
│
└── winget-pkgs\                     ← Forked repo (NEW!)
	├── manifests\                   ← Where packages live
	│   ├── a\
	│   ├── b\
	│   ├── c\
	│   ├── ...
	│   ├── m\Microsoft\...
	│   └── q\                       ← You'll add here!
	├── .github\
	├── Tools\
	└── README.md
```

**✅ Checkpoint:** You should be inside `C:\Users\chris\source\repos\winget-pkgs\`

---

### Step 4: Create Package Folder Structure (2 minutes)

#### 4.1 Create Publisher/Package/Version Folders

The winget repository uses this structure:
```
manifests\[first-letter]\[Publisher]\[PackageName]\[Version]\
```

For ShortCutTool:
- First letter: `q` (Qisuk)
- Publisher: `Qisuk`
- Package: `ShortCutTool`
- Version: `1.0.0`

**Create the folders:**

```powershell
# You should still be in: C:\Users\chris\source\repos\winget-pkgs\

# Create the package directory structure
mkdir manifests\q\Qisuk\ShortCutTool\1.0.0
```

**Your folder structure is now:**

```
C:\Users\chris\source\repos\winget-pkgs\
│
├── manifests\
│   ├── a\
│   ├── b\
│   ├── ...
│   ├── q\                                        ← NEW!
│   │   └── Qisuk\                                ← NEW!
│   │       └── ShortCutTool\                     ← NEW!
│   │           └── 1.0.0\                        ← NEW! (empty for now)
│   └── ...
│
└── [other winget-pkgs files]
```

**✅ Checkpoint:** Folder `manifests\q\Qisuk\ShortCutTool\1.0.0\` exists and is empty

---

### Step 5: Copy Manifest Files (1 minute)

#### 5.1 Copy Your Manifests

```powershell
# Still in: C:\Users\chris\source\repos\winget-pkgs\

# Copy all .yaml files from your project to the version folder
Copy-Item ..\ShortCutTool\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.0.0\
```

#### 5.2 Verify Files Copied

```powershell
Get-ChildItem manifests\q\Qisuk\ShortCutTool\1.0.0\
```

**Expected output:**
```
Name
----
Qisuk.ShortCutTool.installer.yaml
Qisuk.ShortCutTool.locale.en-US.yaml
Qisuk.ShortCutTool.yaml
```

**Your complete folder structure is now:**

```
C:\Users\chris\source\repos\winget-pkgs\
│
├── manifests\
│   ├── [a-p folders]...
│   ├── q\
│   │   └── Qisuk\
│   │       └── ShortCutTool\
│   │           └── 1.0.0\
│   │               ├── Qisuk.ShortCutTool.yaml               ← Version manifest
│   │               ├── Qisuk.ShortCutTool.installer.yaml    ← Installer manifest
│   │               └── Qisuk.ShortCutTool.locale.en-US.yaml ← Locale manifest
│   └── [r-z folders]...
│
└── [other winget-pkgs files]
```

**✅ Checkpoint:** Three `.yaml` files are in the `1.0.0` folder

---

### Step 6: Validate Manifests (2 minutes)

#### 6.1 Run Winget Validation

```powershell
# Still in: C:\Users\chris\source\repos\winget-pkgs\

winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\
```

**Expected output:**
```
Manifest validation succeeded.
```

#### 6.2 If Validation Fails

Common issues:
- **SHA256 placeholder:** Re-run `Create-WingetRelease.ps1`
- **Missing fields:** Check each .yaml file
- **Wrong package ID:** Must be exactly `Qisuk.ShortCutTool`

**✅ Checkpoint:** Validation passes with no errors

---

### Step 7: Create Branch and Commit (3 minutes)

#### 7.1 Create a New Branch

```powershell
# Still in: C:\Users\chris\source\repos\winget-pkgs\

# Create and switch to a new branch
git checkout -b qisuk-shortcuttool-1.0.0
```

#### 7.2 Stage Your Changes

```powershell
# Add only your new package folder
git add manifests\q\Qisuk\ShortCutTool\1.0.0\
```

#### 7.3 Verify What Will Be Committed

```powershell
git status
```

**Expected output:**
```
On branch qisuk-shortcuttool-1.0.0
Changes to be committed:
  (use "git restore --staged <file>..." to unstage)
		new file:   manifests/q/Qisuk/ShortCutTool/1.0.0/Qisuk.ShortCutTool.installer.yaml
		new file:   manifests/q/Qisuk/ShortCutTool/1.0.0/Qisuk.ShortCutTool.locale.en-US.yaml
		new file:   manifests/q/Qisuk/ShortCutTool/1.0.0/Qisuk.ShortCutTool.yaml
```

#### 7.4 Commit Your Changes

For a **new package** (first submission):
```powershell
git commit -m "New package: Qisuk.ShortCutTool version 1.0.0"
```

For a **package update** (future versions):
```powershell
git commit -m "Update: Qisuk.ShortCutTool version 1.1.0"
```

**✅ Checkpoint:** Changes committed to your local branch

---

### Step 8: Push to Your Fork (2 minutes)

#### 8.1 Push the Branch

```powershell
# Still in: C:\Users\chris\source\repos\winget-pkgs\

git push origin qisuk-shortcuttool-1.0.0
```

**Expected output:**
```
Enumerating objects: X, done.
Counting objects: 100% (X/X), done.
...
To https://github.com/YOUR_USERNAME/winget-pkgs.git
 * [new branch]      qisuk-shortcuttool-1.0.0 -> qisuk-shortcuttool-1.0.0
```

**✅ Checkpoint:** Branch pushed to your GitHub fork

---

### Step 9: Create Pull Request (5 minutes)

#### 9.1 Open Pull Request Page

GitHub should show a banner suggesting you create a PR. Click "Compare & pull request"

**Or manually:**

1. Go to: https://github.com/microsoft/winget-pkgs
2. Click "Pull requests" tab
3. Click "New pull request"
4. Click "compare across forks"
5. Set:
   - **base repository:** `microsoft/winget-pkgs`
   - **base branch:** `master`
   - **head repository:** `YOUR_USERNAME/winget-pkgs`
   - **compare branch:** `qisuk-shortcuttool-1.0.0`

#### 9.2 Fill Pull Request Form

**Title (for new package):**
```
New package: Qisuk.ShortCutTool version 1.0.0
```

**Title (for update):**
```
Update: Qisuk.ShortCutTool version 1.1.0
```

**Description:**
```
# Qisuk.ShortCutTool version 1.0.0

Keyboard shortcut manager using Meh and Hyper keys for launching and cycling through application windows.

## Changes
- Initial submission
- Version 1.0.0

## Testing
- [x] Manifests validated locally
- [x] Package tested on Windows 11
- [x] All required fields present
```

#### 9.3 Submit Pull Request

Click "Create pull request"

**✅ Checkpoint:** Pull request created and visible at `https://github.com/microsoft/winget-pkgs/pulls`

---

### Step 10: Wait for Review (2-7 days)

#### 10.1 Automated Checks

Within minutes, automated validation runs:
- Manifest schema validation
- Package hash verification
- Duplicate detection
- Policy compliance

**Watch for:**
- ✅ Green checkmarks = passed
- ❌ Red X's = need fixes

#### 10.2 Manual Review

Winget maintainers will:
- Review your package metadata
- Test installation
- Check for policy violations
- Request changes if needed

#### 10.3 Respond to Feedback

If changes requested:

```powershell
# In: C:\Users\chris\source\repos\winget-pkgs\
# On branch: qisuk-shortcuttool-1.0.0

# Make changes to manifests
notepad manifests\q\Qisuk\ShortCutTool\1.0.0\Qisuk.ShortCutTool.yaml

# Commit and push
git add manifests\q\Qisuk\ShortCutTool\1.0.0\
git commit -m "Update: Address review feedback"
git push origin qisuk-shortcuttool-1.0.0
```

The PR updates automatically!

**✅ Checkpoint:** PR approved and merged by maintainers

---

## 🎉 Success! Package Published

After merge, your package is live within 1-2 hours.

### Users Can Install With:

```powershell
winget search ShortCutTool
winget install Qisuk.ShortCutTool
```

---

## 📊 Complete Final Folder Structure

### Your Project (ShortCutTool)

```
C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\
│
├── .winget\                                      ← Source manifests
│   ├── Qisuk.ShortCutTool.yaml
│   ├── Qisuk.ShortCutTool.installer.yaml
│   └── Qisuk.ShortCutTool.locale.en-US.yaml
│
├── Assets\
│   └── Icons\
│       ├── app.ico                               ← Application icon
│       └── [documentation files]
│
├── release\                                      ← Release artifacts
│   ├── ShortCutTool-v1.0.0.zip                  ← Uploaded to GitHub
│   ├── ShortCutTool-v1.0.0.sha256
│   └── publish\
│       ├── ShortCutTool.exe
│       ├── shortcuts.json
│       └── [runtime files]
│
├── ShortCutTool.csproj
├── Program.cs
├── TrayApplicationContext.cs
├── KeyboardHookService.cs
├── shortcuts.json
├── README.md
├── LICENSE
├── Create-WingetRelease.ps1                     ← Automation script
├── WINGET_SUBMISSION_COMPLETE.md                ← This file
└── [other source files]
```

### winget-pkgs Repository

```
C:\Users\chris\source\repos\winget-pkgs\
│
├── manifests\
│   ├── a\
│   ├── b\
│   ├── ...
│   ├── q\
│   │   └── Qisuk\
│   │       └── ShortCutTool\
│   │           └── 1.0.0\                        ← Your submission
│   │               ├── Qisuk.ShortCutTool.yaml
│   │               ├── Qisuk.ShortCutTool.installer.yaml
│   │               └── Qisuk.ShortCutTool.locale.en-US.yaml
│   └── ...
│
├── .github\
├── Tools\
└── README.md
```

### GitHub Release

```
https://github.com/Qisuk/ShortCutTool/releases/
│
└── v1.0.0
	├── Release notes (from locale.en-US.yaml)
	└── Assets:
		└── ShortCutTool-v1.0.0.zip               ← Public download URL
```

---

## 📝 For Future Version Updates (1.1.0, 1.2.0, etc.)

### Quick Update Process

```powershell
# 1. Update version in ShortCutTool project
# Edit: ShortCutTool.csproj (set Version to 1.1.0)

# 2. Create new release
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.1.0"

# 3. Upload to GitHub releases
# Tag: v1.1.0, Upload: release\ShortCutTool-v1.1.0.zip

# 4. Update winget-pkgs
cd C:\Users\chris\source\repos\winget-pkgs
git checkout master
git pull upstream master
git checkout -b qisuk-shortcuttool-1.1.0

# 5. Create new version folder
mkdir manifests\q\Qisuk\ShortCutTool\1.1.0
Copy-Item ..\ShortCutTool\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.1.0\

# 6. Validate and submit
winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.1.0\
git add manifests\q\Qisuk\ShortCutTool\1.1.0\
git commit -m "Update: Qisuk.ShortCutTool version 1.1.0"
git push origin qisuk-shortcuttool-1.1.0

# 7. Create PR on GitHub
```

Each version gets its own folder:
```
manifests\q\Qisuk\ShortCutTool\
├── 1.0.0\
│   ├── Qisuk.ShortCutTool.yaml
│   ├── Qisuk.ShortCutTool.installer.yaml
│   └── Qisuk.ShortCutTool.locale.en-US.yaml
├── 1.1.0\
│   ├── Qisuk.ShortCutTool.yaml
│   ├── Qisuk.ShortCutTool.installer.yaml
│   └── Qisuk.ShortCutTool.locale.en-US.yaml
└── 1.2.0\
	└── ...
```

---

## 🔧 Troubleshooting

### ❌ "SHA256 hash placeholder found"

**Fix:**
```powershell
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.0.0"
```

### ❌ "Validation failed: Package not found"

**Fix:** Ensure your GitHub release is public and the download URL works

### ❌ "Duplicate package identifier"

**Fix:** You can't change the package ID after first submission. Current ID: `Qisuk.ShortCutTool`

### ❌ "Manifest schema error"

**Fix:** Check YAML formatting - no tabs, proper spacing

### ❌ Git push fails

**Fix:**
```powershell
# Set up upstream if needed
cd C:\Users\chris\source\repos\winget-pkgs
git remote add upstream https://github.com/microsoft/winget-pkgs.git

# Sync your fork
git checkout master
git pull upstream master
git push origin master
```

---

## 📚 Resources

- **Winget Docs:** https://learn.microsoft.com/windows/package-manager/
- **Submission Guidelines:** https://github.com/microsoft/winget-pkgs/blob/master/AUTHORING_MANIFESTS.md
- **Package Repo:** https://github.com/microsoft/winget-pkgs
- **Your Releases:** https://github.com/Qisuk/ShortCutTool/releases

---

## ✅ Submission Checklist

Use this before submitting:

- [ ] Version updated in `ShortCutTool.csproj`
- [ ] `Create-WingetRelease.ps1` executed successfully
- [ ] `release\ShortCutTool-vX.X.X.zip` created
- [ ] GitHub release created with tag `vX.X.X`
- [ ] ZIP uploaded to GitHub release
- [ ] winget-pkgs repository forked
- [ ] winget-pkgs repository cloned locally
- [ ] Version folder created: `manifests\q\Qisuk\ShortCutTool\X.X.X\`
- [ ] Three .yaml files copied to version folder
- [ ] `winget validate` passes
- [ ] Changes committed to new branch
- [ ] Branch pushed to fork
- [ ] Pull request created on microsoft/winget-pkgs
- [ ] Automated checks pass (green checkmarks)

---

**🎉 That's it! You're now a winget package maintainer!**

Questions or issues? File an issue at: https://github.com/Qisuk/ShortCutTool/issues
