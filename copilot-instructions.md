# Copilot Instructions for ShortCutTool

## Documentation File Organization

All markdown (.md) documentation files for this project should be saved to the `Documents/` folder in the solution root.

### Folder Structure

- **Root level** - Solution root directory
  - `copilot-instructions.md` - Copilot coding guidelines and instructions (exempt from Documents folder)
  - `README.md` - Main project documentation (exempt from Documents folder)

- **Documents/** - Main documentation folder (solution root level)
  - `CHANGELOG.md` - Version history and changes
  - Other project documentation files (.md files)

- **Documents/Icons/** - Documentation related to icon assets
  - Icon-specific documentation (README.md, CONCEPTS.md, etc.)

### When Creating New Documentation

1. Always save new `.md` files to the `Documents/` folder
2. **Exception:** `copilot-instructions.md` and `README.md` should remain at the solution root level for standard GitHub/workspace conventions
3. For icon-related documentation, use `Documents/Icons/` subdirectory
4. Update the `ShortCutTool.csproj` file if the documentation file needs to be included in the package

### Project File References

The `ShortCutTool.csproj` file includes the following documentation references:
- `PackageReadmeFile` - Points to `Documents\README.md` for NuGet package metadata

If you create new documentation that should be included in the package, add appropriate `PackagePath` entries to the `.csproj` file.

### Build and Publish Scripts

- `Build-And-Publish.ps1` - Uses documentation for reference
- `Create-WingetRelease.ps1` - May reference documentation files
- `Validate-WingetManifests.ps1` - Validation scripts

Ensure any scripts that reference documentation files use the updated `Documents/` paths.

## Notes

- The `Assets/Icons/` folder now contains only icon files and resources
- Primary documentation (`README.md`, `copilot-instructions.md`) is at the solution root for GitHub conventions
- Additional `.md` files have been consolidated under `Documents/` for better organization
- This improves maintainability and keeps documentation organized by category
