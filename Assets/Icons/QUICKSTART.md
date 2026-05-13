# Icon Creation Quick Start

## 🎯 Current Status
✅ Placeholder icon created (`app.ico`)  
✅ Project configured to use the icon  
✅ Build successful with icon embedded  

## 🚀 Quick Steps to Create Your Custom Icon

### Method 1: AI Generation (Recommended - 5 minutes)

1. **Go to Microsoft Designer or Bing Image Creator**
   - https://designer.microsoft.com/
   - https://www.bing.com/images/create

2. **Copy this prompt:**
   ```
   Modern app icon for keyboard shortcut manager software, flat design style, 
   gradient blue background, white keyboard keys showing Ctrl+Alt symbols, 
   lightning bolt accent, minimalist tech aesthetic, square format with 
   rounded corners, high contrast, 512x512px
   ```

3. **Generate and download** the image (PNG format)

4. **Save it** as `source.png` in `Assets\Icons\`

5. **Convert to .ico:**
   ```powershell
   cd Assets\Icons
   .\ConvertTo-Icon.ps1 -InputImage source.png -OutputIcon app.ico
   ```

6. **Rebuild project:**
   ```powershell
   dotnet build -c Release
   ```

Done! Your custom icon is now embedded in the application.

---

### Method 2: Use Icon Library (2 minutes)

1. **Visit an icon site:**
   - https://iconoir.com/
   - https://lucide.dev/
   - https://heroicons.com/

2. **Search for:** "keyboard", "command", "shortcut", "window"

3. **Download** as SVG or PNG (512px+)

4. **Follow steps 4-6 from Method 1**

---

### Method 3: Design Your Own

Use any design tool:
- **Figma** (free, web-based)
- **Adobe Illustrator**
- **Inkscape** (free desktop app)
- **Canva** (free, web-based)

Design guidelines:
- Square format (512x512 or 1024x1024)
- Simple, bold shapes
- High contrast colors
- Test readability at 16x16 pixels
- Export as PNG with transparency

Then follow steps 4-6 from Method 1.

---

## 📁 What's Already Set Up

```
Assets/Icons/
├── app.ico                    ← Current icon (placeholder - replace this!)
├── README.md                  ← Detailed documentation
├── ICON_PROMPTS.md           ← AI generation prompts
├── ConvertTo-Icon.ps1        ← Conversion script
└── QUICKSTART.md             ← This file!
```

**Project Configuration:**
- ✅ `ShortCutTool.csproj` references `Assets\Icons\app.ico`
- ✅ Icon will be embedded in all builds automatically
- ✅ Works with winget deployment

---

## 🎨 Design Inspiration

The icon should represent:
- **Keyboard shortcuts** (Ctrl, Alt, Shift, Win keys)
- **Speed/Efficiency** (lightning, rocket, quick access)
- **Window management** (multiple windows, app launching)
- **Productivity** (modern, professional, tech-focused)

**Color suggestions:**
- Blue gradient (#0078D4 → #005A9E) - Windows accent
- White/light gray for keys/symbols
- Keep it simple for 16x16 visibility

---

## ✅ Testing Your Icon

After creating/replacing `app.ico`:

1. **Rebuild:**
   ```powershell
   dotnet build -c Release
   ```

2. **Check executable:**
   - Open `bin\Release\net10.0-windows\win-x64\publish\` in File Explorer
   - Look at `ShortCutTool.exe` file icon

3. **Run the app:**
   - Check taskbar icon
   - Check system tray icon
   - Verify it's visible at small sizes

4. **Test winget package:**
   ```powershell
   .\Create-WingetRelease.ps1
   ```

---

## 💡 Tips

- **Start simple** - A basic icon is better than no icon
- **Test at 16x16** - This is the most common size (taskbar/tray)
- **Use high contrast** - Must work on light and dark backgrounds
- **Keep it recognizable** - User should instantly identify your app
- **Iterate** - Easy to replace and rebuild

---

## 🆘 Need Help?

**Common issues:**

**Q: Icon doesn't show after rebuild**
A: Clean and rebuild:
```powershell
dotnet clean
dotnet build -c Release
```

**Q: Convert script fails**
A: Ensure input image is valid PNG/JPG and square format

**Q: Icon looks blurry**
A: Start with higher resolution source (512px minimum)

**Q: Want to change icon later**
A: Just replace `app.ico` and rebuild - that's it!

---

## 📝 Remember

The current placeholder icon is **functional but basic**. Replace it when you have time to create something better - the setup is already done!

For more details, see `README.md` in this directory.
