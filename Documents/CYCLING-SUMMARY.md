# ✅ Cycling Issues Fixed + Reverse Cycling Added!

## Problems Solved

### 1. ✅ Cycling Through Instances Not Working
**Status:** FIXED

**What was wrong:**
- Index tracking started incorrectly
- First window was being skipped
- Cycling wasn't wrapping properly

**What's fixed:**
- Proper index initialization (starts at -1)
- First press now goes to first window
- Correct wrap-around behavior
- Separate handling for 0, 1, and multiple instances

### 2. ✅ Reverse Cycling Added
**Status:** NEW FEATURE

**What you asked for:**
> "each shortcut should allow a 'reverse' option, typically this is based off the Hyper key combination"

**What's implemented:**
- ✅ Hyper key support (Ctrl+Alt+Shift+Win)
- ✅ `useHyperForReverse` configuration option
- ✅ Cycle backwards through instances
- ✅ Works with all configured shortcuts

## Quick Start

### 1. Update Your Config

Edit `publish\shortcuts.json`:

```json
{
  "shortcuts": [
    {
      "key": "C",
      "useMeh": true,
      "useHyperForReverse": true,
      "applicationPath": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
    }
  ]
}
```

### 2. Start the Application

```cmd
cd publish
ShortCutTool.exe
```

### 3. Test It!

**Open 3 Chrome windows, then:**

**Forward:**
- Press `Ctrl+Alt+Shift+C` → Window 1
- Press `Ctrl+Alt+Shift+C` → Window 2
- Press `Ctrl+Alt+Shift+C` → Window 3
- Press `Ctrl+Alt+Shift+C` → Window 1 (cycles)

**Backward:**
- Press `Ctrl+Alt+Shift+Win+C` → Window 3
- Press `Ctrl+Alt+Shift+Win+C` → Window 2
- Press `Ctrl+Alt+Shift+Win+C` → Window 1
- Press `Ctrl+Alt+Shift+Win+C` → Window 3 (cycles)

## Key Changes Made

### Modified Files:

1. **KeyboardHookService.cs**
   - ✅ Added Windows key tracking
   - ✅ Fixed cycling index logic
   - ✅ Added Hyper key detection
   - ✅ Separate handling for 0/1/multiple instances
   - ✅ Forward and reverse cycling
   - ✅ Key suppression to prevent Windows key menu

2. **AppShortcutConfig.cs**
   - ✅ Added `UseHyperForReverse` property

3. **TrayApplicationContext.cs**
   - ✅ Updated to display Hyper key info in shortcuts list

### New Features:

| Feature | Description |
|---------|-------------|
| **Fixed Forward Cycling** | Now properly cycles through all instances |
| **Reverse Cycling** | Cycle backwards with Hyper key |
| **Hyper Key Support** | Ctrl+Alt+Shift+Win detection |
| **Better Index Tracking** | Starts at -1, wraps correctly |
| **Instance Count Handling** | Different logic for 0, 1, or many instances |

## Configuration Options

### New Option: `useHyperForReverse`

```json
{
  "key": "C",
  "useMeh": true,
  "useHyperForReverse": true,  // ← NEW!
  "applicationPath": "C:\\path\\to\\app.exe"
}
```

**Values:**
- `true` - Enable reverse cycling with Hyper key
- `false` (default) - Only forward cycling with Meh key

## Key Combinations

| Name | Keys | Purpose |
|------|------|---------|
| **Meh** | Ctrl + Alt + Shift | Forward cycling |
| **Hyper** | Ctrl + Alt + Shift + Win | Reverse cycling |

## Behavior Matrix

| Instances Running | Meh Action | Hyper Action (if enabled) |
|-------------------|------------|---------------------------|
| 0 | Launch app | Launch app |
| 1 | Bring to front | Bring to front |
| 2+ | Cycle forward | Cycle backward |

## Example Scenarios

### Scenario 1: Working with Multiple Projects in VS Code

You have 4 VS Code windows open for different projects:
- Window 1: Frontend
- Window 2: Backend
- Window 3: Database
- Window 4: Documentation

**Navigate to Backend:**
- Currently on Frontend? Press Meh+V once
- Currently on Documentation? Press Hyper+V three times

**Fast Access:**
- Always press Meh+V repeatedly to cycle forward
- Use Hyper+V to go back if you overshoot

### Scenario 2: Managing Multiple Browser Windows

You have 5 Chrome windows:
- Window 1: Gmail
- Window 2: GitHub
- Window 3: Documentation
- Window 4: YouTube
- Window 5: Shopping

**Quick Navigation:**
- From Gmail to GitHub: Meh+C (once)
- From Shopping back to YouTube: Hyper+C (once)
- From Gmail to Shopping: Hyper+C (once) vs Meh+C (four times)

## Testing Your Setup

### Test Checklist:

1. **No instances:**
   - [ ] Meh+Key launches application
   - [ ] Hyper+Key launches application

2. **One instance:**
   - [ ] Meh+Key brings window to front
   - [ ] Hyper+Key brings window to front

3. **Multiple instances (3+):**
   - [ ] Meh+Key cycles forward (1→2→3→1)
   - [ ] Hyper+Key cycles backward (3→2→1→3)
   - [ ] Can reach any window in both directions

4. **Window states:**
   - [ ] Minimized windows are restored
   - [ ] Hidden windows are brought to front
   - [ ] Focus switches correctly

## Troubleshooting

### "Cycling still doesn't work"
- ✅ This update fixed it! Make sure you're using the new build
- Verify you have multiple windows (not just tabs)
- Check that windows have visible main windows

### "Hyper key doesn't work"
- Make sure `"useHyperForReverse": true` in config
- Press all four modifier keys: Ctrl+Alt+Shift+**Win**
- Try pressing them in order: Ctrl → Alt → Shift → Win → Letter

### "Windows Start Menu opens"
- Try pressing keys in correct order (see above)
- The app tries to suppress it, but timing matters
- Release all keys together after the letter key

### "It skips windows"
- ✅ Fixed in this update!
- If still happening, restart the app

## Performance Notes

- Keyboard hook runs in separate task (non-blocking)
- Instance detection is fast (queries process list)
- Window activation uses Windows APIs (instant)

## Files Modified Summary

```
✅ KeyboardHookService.cs   (major changes - cycling logic)
✅ AppShortcutConfig.cs     (added UseHyperForReverse)
✅ TrayApplicationContext.cs (display Hyper info)
✅ publish/shortcuts.json   (example updated)
```

## Documentation Added

```
📄 CYCLING-FEATURE.md      (detailed feature documentation)
📄 CYCLING-QUICKREF.md     (quick reference card)
📄 CYCLING-SUMMARY.md      (this file)
```

## Next Steps

1. **Stop the old version** (if running)
   ```powershell
   Stop-Process -Name ShortCutTool -Force
   ```

2. **Update your config** (`publish\shortcuts.json`)
   - Add `"useHyperForReverse": true` where desired

3. **Start the new version**
   ```cmd
   cd publish
   ShortCutTool.exe
   ```

4. **Test it out!**
   - Open multiple instances of an application
   - Try forward cycling with Meh key
   - Try reverse cycling with Hyper key

---

## ✅ Summary

**Both issues resolved:**
1. ✅ Forward cycling now works correctly
2. ✅ Reverse cycling with Hyper key implemented

**Your ShortCutTool is now even more powerful! 🚀**

Navigate through your application windows like a pro with bidirectional cycling!
