# 🤖 GitHub Actions - Automated Winget Publishing

## 🎯 Overview

This workflow **automatically submits ShortCutTool to winget** whenever you create a GitHub release.

**No manual steps required!** Just create a release, and the action handles everything.

---

## ✅ What the Action Does

When you publish a GitHub release (e.g., `v1.0.0`), the action automatically:

1. ✅ Builds the project in Release mode
2. ✅ Creates the ZIP package
3. ✅ Calculates SHA256 hash
4. ✅ Updates manifest files with correct hash and download URL
5. ✅ Forks microsoft/winget-pkgs (if not already forked)
6. ✅ Creates the correct folder structure: `manifests\q\Qisuk\ShortCutTool\X.X.X\`
7. ✅ Copies manifest files to the version folder
8. ✅ Validates manifests
9. ✅ Creates a Pull Request to microsoft/winget-pkgs

**Time saved:** 30-45 minutes per release!

---

## 🔧 Setup (One-Time - 5 Minutes)

### Step 1: Create GitHub Personal Access Token

You need a token with permission to create PRs to microsoft/winget-pkgs.

#### 1.1 Generate Token

1. Go to: https://github.com/settings/tokens/new
2. **Note:** `Winget Publisher`
3. **Expiration:** `No expiration` (or your preference)
4. **Select scopes:**
   - ✅ `public_repo` (under `repo`)
   - ✅ `workflow`

5. Click **Generate token**
6. **Copy the token** (you won't see it again!)

#### 1.2 Add Token to Repository Secrets

1. Go to: https://github.com/Qisuk/ShortCutTool/settings/secrets/actions
2. Click **New repository secret**
3. **Name:** `WINGET_TOKEN`
4. **Value:** Paste the token you copied
5. Click **Add secret**

**✅ Setup complete!** The workflow can now create PRs on your behalf.

---

### Step 2: Update Manifest Templates (Already Done!)

The workflow expects placeholders in your manifest files:

**`.winget/Qisuk.ShortCutTool.installer.yaml`:**
```yaml
InstallerUrl: INSERT_DOWNLOAD_URL_HERE
InstallerSha256: INSERT_SHA256_HERE
```

**These are already in your manifests!** ✅

---

## 🚀 How to Use

### Publishing a New Version

#### Option 1: Via GitHub Web UI (Easiest)

1. **Go to releases:**
   ```
   https://github.com/Qisuk/ShortCutTool/releases/new
   ```

2. **Fill in the form:**
   - **Tag:** `v1.0.0` (must start with `v`)
   - **Release title:** `ShortCutTool v1.0.0`
   - **Description:** Copy from `.winget/Qisuk.ShortCutTool.locale.en-US.yaml`

3. **Attach the ZIP file:**
   - Upload `release\ShortCutTool-v1.0.0.zip`
   - (Build it first with `.\Create-WingetRelease.ps1 -Version "1.0.0"`)

4. **Click "Publish release"**

5. **Wait for automation:**
   - Go to: https://github.com/Qisuk/ShortCutTool/actions
   - Watch the "Publish to Winget" workflow run
   - Takes ~5 minutes

6. **Check the PR:**
   - The action creates a PR to microsoft/winget-pkgs
   - You'll get a notification
   - Link will be in the workflow output

**Done!** No manual winget-pkgs cloning or PR creation needed!

---

#### Option 2: Via GitHub CLI

```powershell
# Build first
.\Create-WingetRelease.ps1 -Version "1.0.0"

# Create release with CLI
gh release create v1.0.0 `
  release\ShortCutTool-v1.0.0.zip `
  --title "ShortCutTool v1.0.0" `
  --notes "Release notes here"

# Action runs automatically!
```

---

#### Option 3: Via Git Tags

```powershell
# Build first
.\Create-WingetRelease.ps1 -Version "1.0.0"

# Tag and push
git tag v1.0.0
git push origin v1.0.0

# Then create release from the tag on GitHub
# Upload the ZIP file
# Action runs automatically!
```

---

## 📊 Workflow Steps Explained

### The Workflow File: `.github/workflows/publish-winget.yml`

```yaml
name: Publish to Winget

on:
  release:
	types: [published]  # Triggers when you publish a release
```

**Trigger:** Runs when you click "Publish release" on GitHub

---

### What Happens During the Workflow

```
1. Checkout repository
   ↓
2. Setup .NET 10
   ↓
3. Extract version from tag (v1.0.0 → 1.0.0)
   ↓
4. Build project in Release mode
   ↓
5. Create ZIP package
   ↓
6. Calculate SHA256 hash
   ↓
7. Update manifests with:
   - Real SHA256 hash
   - Real download URL
   - Correct version
   ↓
8. Submit to winget using winget-releaser action:
   - Forks microsoft/winget-pkgs
   - Creates folder: manifests\q\Qisuk\ShortCutTool\1.0.0\
   - Copies manifests
   - Validates
   - Creates PR
   ↓
9. You receive notification with PR link
```

---

## 🔍 Monitoring the Workflow

### View Workflow Runs

1. **Go to Actions tab:**
   ```
   https://github.com/Qisuk/ShortCutTool/actions
   ```

2. **Click on the workflow run** (named after your release)

3. **Expand steps** to see detailed output

### What to Look For

**✅ Green checkmarks** - Everything worked!
```
✓ Build and publish release
✓ Update manifest files
✓ Submit to Winget
```

**❌ Red X** - Something failed:
- Check the step that failed
- Read the error message
- Common issues below

---

## 🐛 Troubleshooting

### ❌ "Error: WINGET_TOKEN secret not found"

**Fix:**
```
1. Go to: https://github.com/Qisuk/ShortCutTool/settings/secrets/actions
2. Add secret named WINGET_TOKEN with your personal access token
```

---

### ❌ "Build failed: .NET 10 SDK not found"

**Fix:** Update workflow file if .NET 10 is not available on GitHub runners:
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
	dotnet-version: '8.0.x'  # Use available version
```

---

### ❌ "Validation failed: Manifest schema error"

**Fix:**
```
1. The manifests have syntax errors
2. Test locally first:
   .\Create-WingetRelease.ps1 -Version "X.X.X"
   winget validate --manifest .winget\
3. Fix errors before creating release
```

---

### ❌ "PR creation failed: Permission denied"

**Fix:**
```
1. Check WINGET_TOKEN has correct scopes:
   - public_repo ✓
   - workflow ✓
2. Token hasn't expired
3. Regenerate token if needed
```

---

### ❌ "Release asset not found: ShortCutTool-vX.X.X.zip"

**Fix:**
```
You must upload the ZIP file to the release:
1. Build locally: .\Create-WingetRelease.ps1 -Version "X.X.X"
2. Upload release\ShortCutTool-vX.X.X.zip to GitHub release
3. Then publish the release
```

---

## 📋 Complete Automated Workflow Example

### Scenario: Releasing v1.0.0

```powershell
# ══════════════════════════════════════════════════════
# STEP 1: Update version in your project (if needed)
# ══════════════════════════════════════════════════════
# Edit ShortCutTool.csproj, set <Version>1.0.0</Version>

# ══════════════════════════════════════════════════════
# STEP 2: Build release package locally
# ══════════════════════════════════════════════════════
cd C:\Users\chris\source\repos\ShortCutTool\ShortCutTool
.\Create-WingetRelease.ps1 -Version "1.0.0"

# This creates:
# - release\ShortCutTool-v1.0.0.zip
# - Updates manifests with placeholder URLs

# ══════════════════════════════════════════════════════
# STEP 3: Commit and push changes (if you made any)
# ══════════════════════════════════════════════════════
git add .
git commit -m "Prepare v1.0.0 release"
git push origin master

# ══════════════════════════════════════════════════════
# STEP 4: Create GitHub release
# ══════════════════════════════════════════════════════
# Via Web UI:
# 1. Go to: https://github.com/Qisuk/ShortCutTool/releases/new
# 2. Tag: v1.0.0
# 3. Upload: release\ShortCutTool-v1.0.0.zip
# 4. Click "Publish release"

# Via CLI:
gh release create v1.0.0 `
  release\ShortCutTool-v1.0.0.zip `
  --title "ShortCutTool v1.0.0" `
  --notes "$(Get-Content .winget\Qisuk.ShortCutTool.locale.en-US.yaml | Select-String -Pattern 'ReleaseNotes:' -Context 0,10)"

# ══════════════════════════════════════════════════════
# STEP 5: Watch automation work! 🤖
# ══════════════════════════════════════════════════════
# 1. Go to: https://github.com/Qisuk/ShortCutTool/actions
# 2. Watch "Publish to Winget" workflow
# 3. Wait ~5 minutes
# 4. Get notification with PR link

# ══════════════════════════════════════════════════════
# STEP 6: Check the PR
# ══════════════════════════════════════════════════════
# 1. Click notification link
# 2. Review PR at microsoft/winget-pkgs
# 3. Respond to any feedback from maintainers
# 4. Wait for merge (2-7 days)

# ══════════════════════════════════════════════════════
# DONE! Your package will be published automatically!
# ══════════════════════════════════════════════════════
```

---

## 🎯 Benefits of Automation

### Manual Process (Old Way)
- ⏱️ 30-45 minutes per release
- ❌ Easy to make mistakes (wrong folder, typos)
- ❌ Need to clone winget-pkgs repo
- ❌ Manual manifest updates
- ❌ Manual hash calculation
- ❌ Manual PR creation

### Automated Process (New Way)
- ⏱️ 5 minutes (mostly waiting)
- ✅ Consistent, no human error
- ✅ No local winget-pkgs clone needed
- ✅ Automatic manifest updates
- ✅ Automatic hash calculation
- ✅ Automatic PR creation
- ✅ Can release from anywhere (mobile, web)

---

## 🔄 Updating to a New Version

### Example: v1.0.0 → v1.1.0

```powershell
# 1. Update version (if needed)
# Edit ShortCutTool.csproj: <Version>1.1.0</Version>

# 2. Build
.\Create-WingetRelease.ps1 -Version "1.1.0"

# 3. Commit changes
git add .
git commit -m "Release v1.1.0"
git push

# 4. Create release on GitHub
gh release create v1.1.0 `
  release\ShortCutTool-v1.1.0.zip `
  --title "ShortCutTool v1.1.0" `
  --notes "Bug fixes and improvements"

# 5. Automation runs automatically!
# 6. New PR created for v1.1.0
```

**That's it!** Each version gets its own folder automatically:
```
manifests\q\Qisuk\ShortCutTool\
├── 1.0.0\
├── 1.1.0\
└── 1.2.0\
```

---

## 📦 Using winget-releaser Action

The workflow uses [`vedantmgoyal2009/winget-releaser`](https://github.com/vedantmgoyal2009/winget-releaser):

**Why this action?**
- ✅ Purpose-built for winget submissions
- ✅ Handles all winget-pkgs folder structure
- ✅ Validates manifests automatically
- ✅ Creates PRs with proper formatting
- ✅ Well-maintained and widely used

**What it does:**
1. Forks microsoft/winget-pkgs (if needed)
2. Creates correct folder structure
3. Copies and validates manifests
4. Creates PR with proper title and description
5. Links PR to your release

---

## 🔐 Security Considerations

### Token Permissions

The `WINGET_TOKEN` needs minimal permissions:
- ✅ `public_repo` - Create PRs to public repos
- ✅ `workflow` - Allow actions to run

**It cannot:**
- ❌ Access private repos
- ❌ Delete anything
- ❌ Modify your repo settings

### Token Storage

- ✅ Stored as encrypted GitHub secret
- ✅ Never exposed in logs
- ✅ Only accessible to your workflows

### Revoking Access

If compromised:
1. Go to: https://github.com/settings/tokens
2. Find `Winget Publisher` token
3. Click **Delete**
4. Generate new token
5. Update `WINGET_TOKEN` secret

---

## 📊 Workflow Comparison

| Action | Manual | Automated |
|--------|--------|-----------|
| **Build release** | Manual command | Automated |
| **Calculate SHA256** | Manual script | Automated |
| **Update manifests** | Manual editing | Automated |
| **Fork winget-pkgs** | Manual on GitHub | Automated |
| **Clone locally** | Required | Not needed |
| **Create folders** | Manual mkdir | Automated |
| **Copy manifests** | Manual copy | Automated |
| **Validate** | Manual winget validate | Automated |
| **Git operations** | Manual add/commit/push | Automated |
| **Create PR** | Manual on GitHub | Automated |
| **Time required** | 30-45 minutes | 5 minutes |
| **Error prone** | Yes | No |
| **Repeatable** | Manual effort | Consistent |

---

## ✅ Checklist Before First Automated Release

```
SETUP (One-time):
 ☐ GitHub personal access token created
 ☐ Token added as WINGET_TOKEN secret
 ☐ Workflow file created (.github/workflows/publish-winget.yml)
 ☐ Manifest files have placeholder URLs

BEFORE EACH RELEASE:
 ☐ Version updated in ShortCutTool.csproj
 ☐ Release notes updated in locale manifest
 ☐ Build tested locally
 ☐ Changes committed and pushed

CREATING RELEASE:
 ☐ Tag format: vX.X.X (with 'v' prefix)
 ☐ ZIP file attached to release
 ☐ Release published (not draft)

AFTER RELEASE:
 ☐ Check workflow run in Actions tab
 ☐ Verify PR created at microsoft/winget-pkgs
 ☐ Respond to any maintainer feedback
```

---

## 🎉 Summary

**You asked:** Can I perform the actions to publish as a GitHub action?

**Answer:** Yes! Now you have:

✅ **Automated workflow** (`.github/workflows/publish-winget.yml`)
✅ **Complete setup guide** (this document)
✅ **One-command release:** Just publish a GitHub release!
✅ **No manual winget-pkgs operations** needed
✅ **Consistent, error-free** submissions
✅ **30-45 minutes saved** per release

**Next steps:**

1. **Setup** (5 min): Create token and add as secret
2. **Test** (10 min): Create a test release (v1.0.0)
3. **Watch** (5 min): See automation work
4. **Done!** Future releases take 5 minutes total

---

## 📚 Additional Resources

- **winget-releaser action:** https://github.com/vedantmgoyal2009/winget-releaser
- **GitHub Actions docs:** https://docs.github.com/actions
- **Creating releases:** https://docs.github.com/repositories/releasing-projects-on-github
- **GitHub tokens:** https://docs.github.com/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token

---

**🤖 The manual 30-minute process is now a 5-minute automated workflow!**

See: [WINGET_SUBMISSION_COMPLETE.md](WINGET_SUBMISSION_COMPLETE.md) for manual process details (as backup)
