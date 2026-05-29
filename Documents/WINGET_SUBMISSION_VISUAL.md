# 📦 Winget Submission - Visual Quick Reference

## 🎯 The Big Picture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      YOUR COMPUTER                                       │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  C:\Users\chris\source\repos\                                           │
│  │                                                                       │
│  ├── ShortCutTool\ShortCutTool\          ← YOUR PROJECT                 │
│  │   ├── .winget\                        ← Manifest source files        │
│  │   │   ├── Qisuk.ShortCutTool.yaml                                    │
│  │   │   ├── Qisuk.ShortCutTool.installer.yaml                          │
│  │   │   └── Qisuk.ShortCutTool.locale.en-US.yaml                       │
│  │   │                                                                   │
│  │   └── release\                        ← Build artifacts              │
│  │       ├── ShortCutTool-v1.0.0.zip     ← Upload to GitHub             │
│  │       └── publish\                                                    │
│  │           └── ShortCutTool.exe                                        │
│  │                                                                       │
│  └── winget-pkgs\                        ← FORKED REPO (microsoft)      │
│      └── manifests\                      ← Package database             │
│          └── q\                          ← First letter                 │
│              └── Qisuk\                  ← Publisher                    │
│                  └── ShortCutTool\       ← Package name                 │
│                      └── 1.0.0\          ← Version                      │
│                          ├── Qisuk.ShortCutTool.yaml                    │
│                          ├── Qisuk.ShortCutTool.installer.yaml          │
│                          └── Qisuk.ShortCutTool.locale.en-US.yaml       │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘

							  │
							  │ git push
							  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                          GITHUB                                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  github.com/Qisuk/ShortCutTool/releases/                                │
│  └── v1.0.0                                                              │
│      └── ShortCutTool-v1.0.0.zip         ← Public download URL          │
│                                                                          │
│  github.com/YOUR_USERNAME/winget-pkgs/   ← Your fork                    │
│  └── branch: qisuk-shortcuttool-1.0.0    ← Your submission branch       │
│                                                                          │
│                              │                                           │
│                              │ Create Pull Request                       │
│                              ▼                                           │
│                                                                          │
│  github.com/microsoft/winget-pkgs/       ← Official repo                │
│  └── Pull Requests                                                       │
│      └── "New package: Qisuk.ShortCutTool version 1.0.0"               │
│                                                                          │
│                              │                                           │
│                              │ Reviewed & Merged                         │
│                              ▼                                           │
│                                                                          │
│  github.com/microsoft/winget-pkgs/manifests/q/Qisuk/ShortCutTool/1.0.0/ │
│  └── [Your manifests are now official!]                                 │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘

							  │
							  │ Published (1-2 hours)
							  ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                      END USER COMPUTER                                   │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  PS C:\> winget search ShortCutTool                                     │
│  PS C:\> winget install Qisuk.ShortCutTool                              │
│                                                                          │
│  ✅ ShortCutTool installed!                                             │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 🗂️ Folder Structure Explained

### The Path Convention

Winget uses a **four-level** folder structure:

```
manifests\[FirstLetter]\[Publisher]\[PackageName]\[Version]\
```

**For ShortCutTool:**

```
manifests\q\Qisuk\ShortCutTool\1.0.0\
		  │  │     │            │
		  │  │     │            └─── Version number
		  │  │     └──────────────── Package name (matches identifier)
		  │  └────────────────────── Publisher (matches identifier)
		  └───────────────────────── First letter of publisher (lowercase)
```

### Required Files in Version Folder

Every version folder **must contain exactly 3 files**:

```
manifests\q\Qisuk\ShortCutTool\1.0.0\
│
├── Qisuk.ShortCutTool.yaml               ← VERSION MANIFEST
│   (Package ID, version, default locale)
│
├── Qisuk.ShortCutTool.installer.yaml    ← INSTALLER MANIFEST
│   (Download URL, SHA256, architecture, installer type)
│
└── Qisuk.ShortCutTool.locale.en-US.yaml ← LOCALE MANIFEST
	(Description, license, release notes, homepage)
```

**File naming:** Must exactly match your package identifier!

```
PackageIdentifier: Qisuk.ShortCutTool
				   ↓
File names: Qisuk.ShortCutTool.*.yaml
```

---

## 🚦 The 10-Step Process (Visual)

```
┌──────────────────────────────────────────────────────────────┐
│ Step 1: BUILD RELEASE                                         │
│ .\Create-WingetRelease.ps1 -Version "1.0.0"                  │
│ ✅ Creates: release\ShortCutTool-v1.0.0.zip                  │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 2: GITHUB RELEASE                                        │
│ Upload ZIP to: github.com/Qisuk/ShortCutTool/releases/new    │
│ ✅ Tag: v1.0.0                                                │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 3: FORK WINGET-PKGS (One-time)                          │
│ Fork: github.com/microsoft/winget-pkgs                        │
│ ✅ Fork: github.com/YOUR_USERNAME/winget-pkgs                │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 4: CREATE FOLDER                                         │
│ mkdir manifests\q\Qisuk\ShortCutTool\1.0.0                   │
│ ✅ Folder created                                             │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 5: COPY MANIFESTS                                        │
│ Copy-Item ...\ShortCutTool\.winget\*.yaml                    │
│           manifests\q\Qisuk\ShortCutTool\1.0.0\              │
│ ✅ 3 files copied                                             │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 6: VALIDATE                                              │
│ winget validate --manifest manifests\q\Qisuk\ShortCutTool\...│
│ ✅ Validation succeeded                                       │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 7: COMMIT                                                │
│ git checkout -b qisuk-shortcuttool-1.0.0                      │
│ git add manifests\q\Qisuk\ShortCutTool\1.0.0\                │
│ git commit -m "New package: Qisuk.ShortCutTool version 1.0.0"│
│ ✅ Changes committed locally                                  │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 8: PUSH                                                  │
│ git push origin qisuk-shortcuttool-1.0.0                      │
│ ✅ Branch pushed to your fork                                 │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 9: PULL REQUEST                                          │
│ Create PR: github.com/microsoft/winget-pkgs/compare          │
│ Title: "New package: Qisuk.ShortCutTool version 1.0.0"       │
│ ✅ PR submitted                                               │
└──────────────────────────────────────────────────────────────┘
						  ↓
┌──────────────────────────────────────────────────────────────┐
│ Step 10: WAIT & RESPOND                                       │
│ - Automated validation (minutes)                              │
│ - Manual review (2-7 days)                                    │
│ - Respond to feedback if needed                               │
│ ✅ Merged and published!                                      │
└──────────────────────────────────────────────────────────────┘
```

---

## 🎯 File Flow Diagram

```
				YOUR PROJECT                    GITHUB                    WINGET REPO

.winget\                                                                  
├── *.yaml ────────────────┐                                             
						   │                                             
						   │  Copy                                       
release\                   │                                             
├── publish\               │                                             
│   └── *.exe ────┐        │                                             
│                 │        │                                             
│   ZIP           │        │                                             
│   ↓             │        │                                             
├── v1.0.0.zip ───┼────────┼─ Upload ──→ Releases/v1.0.0/               
│   (SHA256)      │        │             └── v1.0.0.zip                  
│                 │        │                 (Public URL)                
│                 │        │                                             
└─────────────────┘        └────────────────────────────────────────┐   
																	 │   
																	 │   
															manifests\q\Qisuk\ShortCutTool\1.0.0\
															├── *.yaml ← │
															│             │
															│   git push  │
															│   ↓         │
															└─→ PR ───────┘
																↓
															  Review
																↓
															  Merge
																↓
															PUBLISHED!
```

---

## 📋 One-Page Command Cheat Sheet

```powershell
# ═══════════════════════════════════════════════════════
# STEP 1-2: BUILD & UPLOAD TO GITHUB
# ═══════════════════════════════════════════════════════
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.0.0"
# → Upload release\ShortCutTool-v1.0.0.zip to GitHub releases

# ═══════════════════════════════════════════════════════
# STEP 3: FORK & CLONE (ONE-TIME SETUP)
# ═══════════════════════════════════════════════════════
cd C:\Users\chris\source\repos\
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
cd winget-pkgs

# ═══════════════════════════════════════════════════════
# STEP 4-5: CREATE FOLDER & COPY FILES
# ═══════════════════════════════════════════════════════
mkdir manifests\q\Qisuk\ShortCutTool\1.0.0
Copy-Item ..\ShortCutTool\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.0.0\

# ═══════════════════════════════════════════════════════
# STEP 6: VALIDATE
# ═══════════════════════════════════════════════════════
winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\

# ═══════════════════════════════════════════════════════
# STEP 7-8: COMMIT & PUSH
# ═══════════════════════════════════════════════════════
git checkout -b qisuk-shortcuttool-1.0.0
git add manifests\q\Qisuk\ShortCutTool\1.0.0\
git commit -m "New package: Qisuk.ShortCutTool version 1.0.0"
git push origin qisuk-shortcuttool-1.0.0

# ═══════════════════════════════════════════════════════
# STEP 9: CREATE PR (ON GITHUB)
# ═══════════════════════════════════════════════════════
# https://github.com/microsoft/winget-pkgs
# → New Pull Request → Compare across forks → Create PR
```

---

## 🔑 Key Folder Paths Reference

| What | Path |
|------|------|
| **Your project** | `C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\` |
| **Your manifests** | `C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\.winget\` |
| **Release ZIP** | `C:\Users\chris\source\repos\ShortCutTool\ShortCutTool\release\ShortCutTool-v1.0.0.zip` |
| **winget-pkgs clone** | `C:\Users\chris\source\repos\winget-pkgs\` |
| **Your submission folder** | `C:\Users\chris\source\repos\winget-pkgs\manifests\q\Qisuk\ShortCutTool\1.0.0\` |

---

## 🎓 Understanding the Package Identifier

Your package identifier is: **`Qisuk.ShortCutTool`**

This creates the folder structure:

```
Publisher . PackageName
   ↓           ↓
Qisuk   .  ShortCutTool
   ↓           ↓
   └─── q/ ──→ Qisuk/ ──→ ShortCutTool/
```

**Rules:**
- First part = Publisher (your name/org)
- Second part = Package name
- Separator = `.` (period)
- First letter of publisher = manifest root folder (lowercase)

**Examples:**
- `Microsoft.PowerShell` → `manifests\m\Microsoft\PowerShell\`
- `Google.Chrome` → `manifests\g\Google\Chrome\`
- `Qisuk.ShortCutTool` → `manifests\q\Qisuk\ShortCutTool\`

---

## ✅ Visual Validation Checklist

```
Before creating PR, verify ALL boxes checked:

YOUR PROJECT:
 ✅ release\ShortCutTool-v1.0.0.zip exists
 ✅ .winget\*.yaml files have correct SHA256 (not placeholder)
 ✅ GitHub release v1.0.0 published
 ✅ ZIP file uploaded to GitHub release

WINGET-PKGS REPO:
 ✅ Folder created: manifests\q\Qisuk\ShortCutTool\1.0.0\
 ✅ Three .yaml files in version folder
 ✅ winget validate passes (no errors)
 ✅ Branch created: qisuk-shortcuttool-1.0.0
 ✅ Changes committed and pushed

GITHUB:
 ✅ Fork exists: github.com/YOUR_USERNAME/winget-pkgs
 ✅ Branch visible in your fork
 ✅ Ready to create PR
```

---

**🎉 Print this page and follow along step-by-step!**

Full details: `WINGET_SUBMISSION_COMPLETE.md`
