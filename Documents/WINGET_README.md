# 📦 Winget Submission Documentation

## 🎯 Complete Guide with Exact Folder Structure

This documentation provides **crystal-clear instructions** for submitting ShortCutTool to the Windows Package Manager (winget) repository.

**The folder structure is shown at EVERY step!**

---

## 🤖 NEW: Fully Automated Publishing!

**⚡ Skip the manual process entirely!**

👉 **[GITHUB_ACTIONS_WINGET.md](GITHUB_ACTIONS_WINGET.md)** ← **Automated Workflow**

**What you get:**
- ✅ Automatic winget submission via GitHub Actions
- ✅ Just create a GitHub release → PR created automatically
- ✅ 5 minutes per release (instead of 30-45 minutes)
- ✅ No manual winget-pkgs operations
- ✅ Zero human error

**Setup:** 5 minutes one-time (create GitHub token + add secret)

---

## 🚀 Quick Start

### 👉 **New to Winget? Start Here:**

**Open:** [`WINGET_START_HERE.md`](WINGET_START_HERE.md)

This file helps you choose the right guide based on your experience level.

---

## 📚 Documentation Files

### Core Guides

| File | Purpose | When to Use |
|------|---------|-------------|
| **[WINGET_START_HERE.md](WINGET_START_HERE.md)** | **Decision guide** | Start here to choose your path |
| **[WINGET_SUBMISSION_COMPLETE.md](WINGET_SUBMISSION_COMPLETE.md)** | **Complete step-by-step guide** | First submission or need detailed help |
| **[WINGET_SUBMISSION_VISUAL.md](WINGET_SUBMISSION_VISUAL.md)** | **Visual reference** | Follow along with diagrams |
| **[WINGET_QUICKSTART.md](WINGET_QUICKSTART.md)** | **Quick commands** | Already know the process |
| **[WINGET_INDEX.md](WINGET_INDEX.md)** | **Navigation hub** | Find the right documentation |

### Supporting Files

| File | Purpose |
|------|---------|
| [WINGET_SUMMARY.md](WINGET_SUMMARY.md) | Overview of what was created |
| [WINGET_DEPLOYMENT_READY.md](WINGET_DEPLOYMENT_READY.md) | Deployment readiness checklist |
| [Create-WingetRelease.ps1](Create-WingetRelease.ps1) | Automation script |

---

## 🗂️ The Folder Structure (Critical!)

**Your winget submission must use this exact structure:**

```
C:\Users\chris\source\repos\winget-pkgs\
│
└── manifests\
	└── q\                          ← First letter of "Qisuk"
		└── Qisuk\                  ← Publisher name
			└── ShortCutTool\       ← Package name
				└── 1.0.0\          ← Version number
					├── Qisuk.ShortCutTool.yaml
					├── Qisuk.ShortCutTool.installer.yaml
					└── Qisuk.ShortCutTool.locale.en-US.yaml
```

**The complete guide shows this structure at every step with verification commands!**

---

## ⚡ 30-Second Overview

### The 10-Step Process

1. **Build** release with script
2. **Upload** ZIP to GitHub
3. **Fork** winget-pkgs repo
4. **Create** version folder
5. **Copy** manifest files
6. **Validate** with winget CLI
7. **Commit** changes
8. **Push** to your fork
9. **Create** Pull Request
10. **Wait** for approval (2-7 days)

**Time:** 30-45 minutes (first time), 15 minutes (updates)

---

## 📋 Essential Commands

```powershell
# Build release
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.0.0"

# Setup (one-time)
cd C:\Users\chris\source\repos\
git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
cd winget-pkgs

# Create submission
mkdir manifests\q\Qisuk\ShortCutTool\1.0.0
Copy-Item ..\ShortCutTool\ShortCutTool\.winget\*.yaml manifests\q\Qisuk\ShortCutTool\1.0.0\

# Validate and submit
winget validate --manifest manifests\q\Qisuk\ShortCutTool\1.0.0\
git checkout -b qisuk-shortcuttool-1.0.0
git add manifests\q\Qisuk\ShortCutTool\1.0.0\
git commit -m "New package: Qisuk.ShortCutTool version 1.0.0"
git push origin qisuk-shortcuttool-1.0.0

# Create PR on: github.com/microsoft/winget-pkgs
```

**Full context:** See the complete guide!

---

## 🎯 What Makes This Documentation Different

### ✅ Crystal Clear Folder Structure
- **Exact paths** at every step
- **Visual diagrams** showing file flow
- **Verification commands** to check you're on track

### ✅ Multiple Learning Paths
- **Complete guide** for first-timers
- **Visual reference** for following along
- **Quick commands** for experienced users

### ✅ No Guessing
- **Checkpoints** after every step
- **Troubleshooting** for common issues
- **Before/after** folder structure shown

---

## 📖 Recommended Reading Order

### For First Submission

1. **Start:** `WINGET_START_HERE.md` (2 min)
2. **Read:** `WINGET_SUBMISSION_COMPLETE.md` (30 min)
3. **Reference:** `WINGET_SUBMISSION_VISUAL.md` (keep open)
4. **Execute:** Follow steps 1-10 from complete guide
5. **Verify:** Checklist at end of complete guide

### For Updates

1. **Quick reference:** `WINGET_QUICKSTART.md`
2. **Update version** in project
3. **Run script:** `Create-WingetRelease.ps1`
4. **Follow** steps 4-9 from quickstart

---

## ✅ Pre-Submission Checklist

```
☐ Folder: manifests\q\Qisuk\ShortCutTool\1.0.0\ exists
☐ Three .yaml files copied to version folder
☐ winget validate passes
☐ GitHub release v1.0.0 created with ZIP
☐ Changes committed to branch
☐ Branch pushed to fork

Ready to create Pull Request!
```

---

## 🎓 Key Concepts

### Package Identifier
```
Qisuk.ShortCutTool
  ↓         ↓
Publisher   PackageName
```

### Folder Convention
```
manifests\[FirstLetter]\[Publisher]\[PackageName]\[Version]\
		  ↓             ↓           ↓              ↓
manifests\q\Qisuk\ShortCutTool\1.0.0\
```

### Required Files
```
1. Qisuk.ShortCutTool.yaml               (Version manifest)
2. Qisuk.ShortCutTool.installer.yaml     (Installer + SHA256)
3. Qisuk.ShortCutTool.locale.en-US.yaml  (Metadata)
```

**All must be in the version folder and match the package identifier!**

---

## 🆘 Need Help?

- **Unclear folder structure?** → `WINGET_SUBMISSION_VISUAL.md` (diagrams)
- **Step-by-step guidance?** → `WINGET_SUBMISSION_COMPLETE.md` (detailed)
- **Quick command lookup?** → `WINGET_QUICKSTART.md` (commands only)
- **Not sure where to start?** → `WINGET_START_HERE.md` (decision guide)

---

## 🎉 Success Criteria

After following the guides:

✅ **You understand** the folder structure  
✅ **You can create** the correct directory layout  
✅ **You can validate** manifests before submitting  
✅ **You can submit** a Pull Request  
✅ **You can update** for future versions  

---

## 🔗 External Resources

- **Winget Docs:** https://learn.microsoft.com/windows/package-manager/
- **Package Repo:** https://github.com/microsoft/winget-pkgs
- **Your Project:** https://github.com/Qisuk/ShortCutTool
- **Your Releases:** https://github.com/Qisuk/ShortCutTool/releases

---

## 📊 Documentation Stats

| Metric | Count |
|--------|-------|
| **Total documentation files** | 8 files |
| **Total lines** | 2,500+ lines |
| **Folder structure diagrams** | Multiple per file |
| **Code examples** | Complete command sets |
| **Troubleshooting scenarios** | All common issues |

---

## 🚀 Your Next Action

**Open:** [`WINGET_START_HERE.md`](WINGET_START_HERE.md)

This file will guide you to the right documentation based on your needs.

**The folder structure is now crystal clear at every step!** 🎯

---

**Questions or issues?** File an issue at: https://github.com/Qisuk/ShortCutTool/issues
