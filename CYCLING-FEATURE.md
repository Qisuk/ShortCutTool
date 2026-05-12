# Cycling and Reverse Cycling Features

## What Was Fixed and Added

### 1. Fixed: Instance Cycling Not Working
**Problem:** Cycling through multiple instances of an application wasn't working properly.

**Solution:** 
- Fixed the cycling logic to properly track the current instance index
- Now starts from index -1 to ensure the first press brings up the first window
- Handles single instance case separately (just brings window to front)
- Multiple instances cycle correctly on each key press

### 2. New: Reverse Cycling with Hyper Key
**Feature:** You can now cycle backwards through instances using the Hyper key combination.

**What is Hyper?**
- **Hyper** = Ctrl + Alt + Shift + Win (all four modifiers)
- **Meh** = Ctrl + Alt + Shift (three modifiers)

**How It Works:**
- **Meh + Key** (Ctrl+Alt+Shift+C): Cycle forward through instances
- **Hyper + Key** (Ctrl+Alt+Shift+Win+C): Cycle backward through instances

## Configuration

Add `"useHyperForReverse": true` to your shortcuts in `shortcuts.json`:

```json
{
  "shortcuts": [
    {
      "key": "C",
      "useMeh": true,
      "useHyperForReverse": true,
      "applicationPath": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
    },
    {
      "key": "V",
      "useMeh": true,
      "useHyperForReverse": true,
      "applicationPath": "C:\\Users\\YourName\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"
    }
  ]
}
```

### Configuration Options

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `key` | string | required | The key to trigger the shortcut (e.g., "C", "V") |
| `useMeh` | boolean | true | Enable Meh key (Ctrl+Alt+Shift) for forward cycling |
| `useHyperForReverse` | boolean | false | Enable Hyper key (Ctrl+Alt+Shift+Win) for reverse cycling |
| `applicationPath` | string | required | Full path to the application executable |
| `workingDirectory` | string | optional | Working directory for the application |

## Usage Examples

### Example 1: Chrome with Multiple Windows

You have 3 Chrome windows open:

1. **First press** (Meh+C): Switches to Window 1
2. **Second press** (Meh+C): Switches to Window 2
3. **Third press** (Meh+C): Switches to Window 3
4. **Fourth press** (Meh+C): Cycles back to Window 1

**Going Backwards:**
1. Currently on Window 3
2. **Press** (Hyper+C): Switches to Window 2
3. **Press** (Hyper+C): Switches to Window 1
4. **Press** (Hyper+C): Cycles back to Window 3

### Example 2: VS Code with Multiple Projects

You have 4 VS Code windows open (different projects):

**Forward:**
- Meh+V → Project 1
- Meh+V → Project 2
- Meh+V → Project 3
- Meh+V → Project 4
- Meh+V → back to Project 1

**Backward:**
- Hyper+V → Project 4
- Hyper+V → Project 3
- Hyper+V → Project 2
- Hyper+V → Project 1
- Hyper+V → back to Project 4

## How Cycling Works Now

### Scenario 1: No Instances Running
- Press shortcut → Application launches
- Index set to 0

### Scenario 2: One Instance Running
- Press shortcut → Window brought to front
- Index remains 0
- No cycling needed

### Scenario 3: Multiple Instances Running
- Press shortcut → Cycles to next window
- Index increments (or decrements for reverse)
- Wraps around at beginning/end

### Key Improvements Made:

1. **Proper Index Tracking**
   - Starts from -1 so first press goes to index 0
   - Properly wraps around in both directions
   - Handles edge cases (0 instances, 1 instance)

2. **Separate Handling**
   - Different logic for 0, 1, and multiple instances
   - Cleaner, more predictable behavior

3. **Windows Key Detection**
   - Added tracking for Left/Right Windows keys
   - Distinguishes between Meh and Hyper key combinations
   - Key suppression to prevent Windows key side effects

## Key Combinations Reference

| Combination | Modifiers | Use Case |
|-------------|-----------|----------|
| **Meh** | Ctrl + Alt + Shift | Forward cycling, launch app |
| **Hyper** | Ctrl + Alt + Shift + Win | Reverse cycling (if enabled) |

## Testing Your Setup

1. **Open multiple instances** of an application (e.g., Chrome, VS Code, Notepad)
2. **Test forward cycling:**
   - Press Ctrl+Alt+Shift+[your key] multiple times
   - Should cycle through all windows
3. **Test reverse cycling:**
   - Press Ctrl+Alt+Shift+Win+[your key] multiple times
   - Should cycle backwards through all windows

## Troubleshooting

### Forward cycling doesn't work
- ✅ Fixed in this update!
- Make sure you have multiple instances with visible windows
- Minimized windows are included (they'll be restored)

### Reverse cycling doesn't work
- Check that `"useHyperForReverse": true` is in your config
- Make sure you're pressing all four modifier keys (Ctrl+Alt+Shift+Win)
- On some keyboards, Win key combinations might be disabled

### Windows key opens Start Menu
- The app attempts to suppress the key
- If it still happens, press and release keys in this order:
  1. Hold Ctrl
  2. Hold Alt
  3. Hold Shift
  4. Hold Win
  5. Press letter key
  6. Release all keys

### Instance doesn't switch
- Verify the application has a main window (not just tray icon)
- Some applications hide their main window - these won't be detected
- Check that the process name matches (see next section)

## Process Name Matching

The app uses the executable filename (without extension) to find processes:

- `chrome.exe` → looks for processes named "chrome"
- `Code.exe` → looks for processes named "Code"
- `devenv.exe` → looks for processes named "devenv"

Make sure your `applicationPath` points to the actual executable.

## Technical Details

### Changes Made to KeyboardHookService.cs:

1. Added `WM_KEYUP` and `WM_SYSKEYUP` constants
2. Added `_winPressed` field for Windows key tracking
3. Modified `HookCallback` to:
   - Track Windows key press/release
   - Detect Hyper key combination
   - Suppress keys when shortcut is activated
   - Handle key release events properly
4. Modified `HandleShortcut` to:
   - Accept `reverse` parameter
   - Fix cycling logic (start from -1)
   - Handle 0, 1, and multiple instance cases
   - Cycle backwards when reverse=true
   - Show error messages via MessageBox

### Changes Made to AppShortcutConfig.cs:

1. Added `UseHyperForReverse` boolean property to `ShortcutMapping`

### Changes Made to TrayApplicationContext.cs:

1. Updated `ShowShortcutInformation` to display Hyper key info when enabled

---

**Enjoy your improved window cycling! 🎉**

Now you can efficiently navigate between multiple instances of your applications, both forwards and backwards!
