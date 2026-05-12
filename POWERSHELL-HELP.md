# PowerShell Script Execution Issue - Solutions

If you get the error: **"cannot be loaded because running scripts is disabled on this system"**

## Quick Solutions

### Solution 1: Use Batch Files Instead (No PowerShell Policy Change Needed)

Use the `.bat` files instead of `.ps1` files:

**Build:**
```cmd
Build-And-Publish.bat
```

**Setup Auto-Start:**
```cmd
Setup-AutoStart.bat publish\ShortCutTool.exe
```

These batch files work without any PowerShell execution policy changes!

---

### Solution 2: Allow Local Scripts (Recommended)

Open PowerShell **as Administrator** and run:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

Then you can use the `.ps1` scripts normally:
```powershell
.\Build-And-Publish.ps1
.\Setup-AutoStart.ps1 -ExePath ".\publish\ShortCutTool.exe"
```

---

### Solution 3: Bypass Policy for Single Script

Run scripts with bypass (no policy change needed):

```powershell
powershell -ExecutionPolicy Bypass -File .\Build-And-Publish.ps1
powershell -ExecutionPolicy Bypass -File .\Setup-AutoStart.ps1 -ExePath ".\publish\ShortCutTool.exe"
```

---

### Solution 4: Manual Steps (No Scripts)

If you prefer not to use any scripts:

**1. Build:**
```powershell
dotnet publish -c Release -o .\publish
```

**2. Create Shortcut:**
- Navigate to the `publish` folder
- Right-click `ShortCutTool.exe` → Create shortcut

**3. Add to Startup:**
- Press `Win+R`, type `shell:startup`, press Enter
- Copy the shortcut into this folder

**4. Configure:**
- Edit `publish\shortcuts.json` with your application paths

**5. Start:**
- Double-click `ShortCutTool.exe` or restart your computer

---

## Which Solution Should I Use?

- ✅ **Easiest**: Solution 1 (Use .bat files)
- ✅ **Best for permanent fix**: Solution 2 (Change policy once)
- ✅ **Quick one-time use**: Solution 3 (Bypass)
- ✅ **Most control**: Solution 4 (Manual)

## Understanding PowerShell Execution Policies

- **Restricted**: No scripts (Windows default)
- **RemoteSigned**: Local scripts OK, downloaded must be signed (recommended)
- **Bypass**: All scripts allowed temporarily

## Still Having Issues?

Use the batch files or manual steps - they always work!
