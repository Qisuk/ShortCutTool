# Application Icons

## Required Icon Files

Place your icon files here with these names:

### 1. **app.ico** (Required for Windows application)
- Multi-resolution .ico file containing:
  - 16x16 pixels
  - 32x32 pixels
  - 48x48 pixels
  - 256x256 pixels (for Windows 10/11)
- Transparent background recommended
- Used for: Executable file, taskbar, window title bar

### 2. **app.png** (Optional but recommended)
- 512x512 or 1024x1024 pixels
- Transparent background (PNG with alpha channel)
- High-resolution source for README, website, etc.

### 3. **app-tray.ico** (Optional - for custom tray icon)
- 16x16 pixels (primary size for system tray)
- Can include 32x32 for high-DPI displays
- Simple, recognizable at tiny size

## Design Guidelines

### For ShortCutTool:
- **Theme**: Keyboard shortcuts, productivity, speed
- **Style**: Modern, flat, minimalist
- **Colors**: 
  - Primary: Blue (#0078D4 - Windows accent blue)
  - Accent: White/Gray for contrast
  - Background: Transparent or solid
- **Elements**: 
  - Keyboard keys (Ctrl, Alt, Shift, Win)
  - Abstract shortcut symbol
  - Window/app launching concept

### Icon Design Tips:
1. **Simple is better** - Must be recognizable at 16x16 pixels
2. **High contrast** - Stands out against light and dark backgrounds
3. **Unique silhouette** - Distinguishable from other apps
4. **No fine details** - Bold shapes and colors
5. **Test at all sizes** - Especially 16x16 (taskbar/tray)

## Quick Creation Options

### Option 1: AI Generation (5 minutes)
1. Go to https://designer.microsoft.com/ or https://www.bing.com/images/create
2. Use this prompt:
   ```
   Simple modern app icon for keyboard shortcut manager, 
   flat design, blue gradient, white keyboard keys showing 
   Ctrl+Alt+Shift, minimalist tech style, square format, 
   transparent background
   ```
3. Download the image
4. Convert to .ico using online tool or script (see below)

### Option 2: From Icon Library (2 minutes)
1. Visit https://iconoir.com/ or https://lucide.dev/
2. Search: "keyboard", "command", "terminal", "shortcut"
3. Download SVG
4. Convert to .ico (see conversion tools below)

### Option 3: Use Placeholder (Temporary)
A basic keyboard icon is included as a placeholder until you create a custom one.

## Converting Images to .ico

### Online Tools (Easy):
- https://convertio.co/png-ico/
- https://icoconvert.com/
- https://redketchup.io/icon-converter

Make sure to select "Include multiple sizes" option!

### PowerShell Script (Local):
See `ConvertTo-Icon.ps1` in this directory for automated conversion.

### Using Visual Studio:
1. Right-click project → Add → New Item → Icon File (.ico)
2. Use built-in icon editor

## After Creating Your Icon

1. **Save as `app.ico`** in this directory
2. **Rebuild project** - Icon will be embedded in executable and copied to output
3. **Test it**:
   - Check executable file icon in File Explorer
   - Check taskbar icon when running
   - **Check system tray icon** - Now uses your custom icon!

## Current Configuration

The `ShortCutTool.csproj` file is configured to:
```xml
<ApplicationIcon>Assets\Icons\app.ico</ApplicationIcon>
```

And the icon file is copied to the output directory:
```xml
<None Update="Assets\Icons\app.ico">
  <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</None>
```

The tray icon loads automatically from the application in this order:
1. From the embedded .exe icon (primary method)
2. From `Assets\Icons\app.ico` in the output directory (development fallback)
3. From system default icon (final fallback)

Once you add `app.ico` to this directory, rebuild the project and both the application icon AND the system tray icon will automatically update!

## Icon Ideas for ShortCutTool

### Concept 1: Keyboard Keys
```
┌─────┬─────┬─────┐
│ Ctrl│ Alt │Shift│
└─────┴─────┴─────┘
```

### Concept 2: Lightning Keyboard
Keyboard with lightning bolt overlay (speed/shortcut)

### Concept 3: Rocket Launch
Rocket + keyboard keys (launching apps)

### Concept 4: Abstract "S" 
Stylized S shape made from keyboard keys or circuit patterns

### Concept 5: Window Grid
Grid of windows with keyboard keys in center (window management)

## Need Help?

If you create a PNG but need help converting to .ico with multiple sizes, let me know and I can provide a PowerShell script to automate the conversion!
