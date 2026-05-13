# AI Icon Generation Prompts

Copy and paste these prompts into AI image generators like Microsoft Designer, Bing Image Creator, DALL-E, or Midjourney.

## Recommended Prompt (Best for ShortCutTool)

```
Modern app icon for keyboard shortcut manager software, flat design style, 
gradient blue background, white keyboard keys showing Ctrl+Alt symbols, 
lightning bolt accent, minimalist tech aesthetic, square format with 
rounded corners, high contrast, transparent or solid background, 512x512px
```

## Alternative Prompts

### Option 1: Keyboard Focus
```
Flat icon design for Windows application, three keyboard keys stacked 
(Ctrl, Alt, Shift), modern blue gradient, white keys with shadow, 
minimalist style, app icon format, 512x512 pixels
```

### Option 2: Window Management Theme
```
Modern app icon showing overlapping windows with keyboard shortcut symbol, 
blue and white color scheme, flat design, tech startup aesthetic, 
rounded square format, high contrast for small sizes
```

### Option 3: Speed/Launch Theme
```
App icon with keyboard and rocket launch symbol, flat minimalist design, 
blue gradient background, white icons, modern tech style, represents 
quick app launching, square format 512x512px
```

### Option 4: Abstract Geometric
```
Abstract geometric app icon representing keyboard shortcuts, 
interconnected keys pattern, modern blue color palette with white accents, 
flat design, minimalist, professional software icon style
```

### Option 5: Circuit Board Tech
```
Tech icon combining keyboard key outline with circuit board pattern, 
modern gradient blue background, white line art, flat design, 
represents productivity software, 512x512 square format
```

## Tips for Best Results

1. **Start with highest resolution** - Ask for 512x512 or 1024x1024
2. **Request transparent background** - Makes icon versatile
3. **Keep it simple** - Will be viewed at 16x16 pixels minimum
4. **High contrast colors** - Blue/white works on all backgrounds
5. **Test multiple prompts** - Generate 3-4 variations and pick the best

## After Generation

1. Download the image (preferably PNG with transparency)
2. If needed, use an image editor to:
   - Remove background (if not transparent)
   - Adjust colors/contrast
   - Crop to perfect square
3. Save as `source.png` in this directory
4. Run the conversion script:
   ```powershell
   .\ConvertTo-Icon.ps1 -InputImage source.png -OutputIcon app.ico
   ```
5. Rebuild the project!

## Free Online Tools

If AI generation doesn't work well, try these icon resources:

- **Iconoir**: https://iconoir.com/ (Free, clean SVG icons)
- **Lucide**: https://lucide.dev/ (Free, consistent icon set)
- **Heroicons**: https://heroicons.com/ (Free, by Tailwind team)
- **Flaticon**: https://flaticon.com/ (Free & premium icons)

Download as SVG or PNG (512px+), then convert using the PowerShell script.
