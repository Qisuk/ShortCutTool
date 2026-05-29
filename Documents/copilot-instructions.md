# Copilot Instructions for ShortCutTool

## Documentation File Organization

All markdown (.md) documentation files for this project should be saved to the `Documents/` folder in the solution root.

### Folder Structure

- **Documents/** - Main documentation folder (solution root level)
  - `copilot-instructions.md` - This file with coding guidelines and instructions
  - `README.md` - Main project documentation
  - `CHANGELOG.md` - Version history and changes
  - Other project documentation files (.md files)

- **Documents/Icons/** - Documentation related to icon assets
  - Icon-specific documentation (README.md, CONCEPTS.md, etc.)

### When Creating New Documentation

1. Always save new `.md` files to the `Documents/` folder
2. For icon-related documentation, use `Documents/Icons/` subdirectory
3. Update the `ShortCutTool.csproj` file if the documentation file needs to be included in the package

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
- All `.md` files have been consolidated under `Documents/` for better organization
- This improves maintainability and keeps documentation separate from code
