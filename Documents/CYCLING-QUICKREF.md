# Quick Reference: Window Cycling

## Keyboard Shortcuts

### Meh Key (Forward Cycling)
**Ctrl + Alt + Shift + [Letter]**

Example: `Ctrl + Alt + Shift + C` for Chrome

### Hyper Key (Reverse Cycling)
**Ctrl + Alt + Shift + Win + [Letter]**

Example: `Ctrl + Alt + Shift + Win + C` for Chrome

## Behavior

| Scenario | Meh Key Action | Hyper Key Action |
|----------|----------------|------------------|
| **No instances running** | Launch application | Launch application |
| **One instance running** | Bring to front | Bring to front |
| **Multiple instances** | Cycle forward (1→2→3→1) | Cycle backward (3→2→1→3) |

## Quick Config Example

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

## Testing Checklist

- [ ] Open 3+ instances of the same application
- [ ] Test Meh key cycles forward through all windows
- [ ] Test Hyper key cycles backward through all windows
- [ ] Test launching when no instances are running
- [ ] Test bringing to front when only 1 instance is running

## Tips

💡 **Muscle Memory**: Use Meh for "next" and Hyper for "previous"

💡 **Windows Key**: Some keyboards have a "Game Mode" that disables the Windows key - make sure it's enabled

💡 **Hold Order**: Press Ctrl→Alt→Shift→Win, then tap your letter key

💡 **Works With**: Any application that has visible windows (Chrome, Firefox, VS Code, File Explorer, etc.)

---

**Need more info?** See [CYCLING-FEATURE.md](CYCLING-FEATURE.md)
