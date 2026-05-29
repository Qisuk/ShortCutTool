# Security Validation and Testing Documentation

## Overview
JSON configuration validation has been added to ShortCutTool to prevent injection attacks and ensure data integrity.

## Implementation

### ConfigValidator.cs
A new static class that provides comprehensive validation for the configuration file.

#### Key Features:
- **Null/Empty Checks**: Validates that required fields are not null or empty
- **Key Validation**: Ensures keys are single alphanumeric characters (A-Z, 0-9)
- **Path Validation**: Prevents path traversal attacks and validates Windows paths
- **Duplicate Detection**: Identifies duplicate shortcut keys (case-insensitive)
- **Size Limits**: Enforces maximum path length (500 characters) and maximum shortcuts (100)
- **Path Sanitization**: Expands environment variables and normalizes paths

#### Dangerous Patterns Blocked:
- Directory traversal (`..\\`, `../`)
- Invalid Windows filename characters (`<`, `>`, `|`, `"`)
- Control characters (0x00-0x1F)
- Reserved Windows device names (CON, PRN, AUX, NUL, COM1-9, LPT1-9)
- UNC network paths (`\\\\server\\share`)

### Integration Points

#### Program.cs (Startup Validation)
```csharp
// Deserialize JSON
var config = JsonSerializer.Deserialize<AppShortcutConfig>(configJson, ...);

// Validate configuration
var validationResult = ConfigValidator.ValidateConfig(config);
if (!validationResult.IsValid)
{
    // Show error message and exit
    MessageBox.Show($"Configuration validation failed:\\n\\n{validationResult.ErrorMessage}...");
    return;
}

// Sanitize paths
foreach (var shortcut in config.Shortcuts)
{
    shortcut.ApplicationPath = ConfigValidator.SanitizePath(shortcut.ApplicationPath);
    shortcut.WorkingDirectory = ConfigValidator.SanitizePath(shortcut.WorkingDirectory);
}
```

#### TrayApplicationContext.cs (GUI Validation)
```csharp
// Validate key
var keyValidation = ConfigValidator.ValidateKey(key);
if (!keyValidation.IsValid)
{
    MessageBox.Show(keyValidation.ErrorMessage, "Invalid Key", ...);
    return;
}

// Validate path
var pathValidation = ConfigValidator.ValidatePath(path, "Application path");
if (!pathValidation.IsValid)
{
    MessageBox.Show(pathValidation.ErrorMessage, "Invalid Path", ...);
    return;
}

// Validate working directory
if (!string.IsNullOrEmpty(workDir))
{
    var workDirValidation = ConfigValidator.ValidateWorkingDirectory(workDir);
    if (!workDirValidation.IsValid)
    {
        MessageBox.Show(workDirValidation.ErrorMessage, "Invalid Working Directory", ...);
        return;
    }
}
```

## Test Scenarios

### Key Validation Tests

#### Valid Keys:
- Single letters: "A", "Z", "a", "z"
- Single digits: "0"-"9"

#### Invalid Keys:
- Null or empty: `null`, "", "   "
- Multiple characters: "AB", "ABC", "12"
- Special characters: "!", "@", "#", " ", "-", "_"

### Path Validation Tests

#### Valid Paths:
- Absolute Windows paths: `C:\\Program Files\\App\\app.exe`
- Environment variables: `%ProgramFiles%\\App\\app.exe`, `%LOCALAPPDATA%\\Programs\\tool.exe`

#### Invalid Paths:
- Null or empty: `null`, "", "   "
- Too long: Paths exceeding 500 characters
- Directory traversal: `..\\..\\malicious\\file.exe`, `C:\\Temp\\..\\..\\Windows\\System32\\cmd.exe`
- Invalid characters: `C:\\Program<>Files\\app.exe`, `C:\\Temp\\file|name.exe`
- Reserved device names: `C:\\CON\\app.exe`, `C:\\PRN\\file.exe`, `C:\\COM1\\app.exe`
- Control characters: `C:\\Temp\\\x00file.exe`
- Relative paths: `relative\\path\\app.exe`, `file.exe`

### Shortcut Validation Tests

#### Valid Shortcut:
```json
{
  "key": "A",
  "applicationPath": "C:\\\\Program Files\\\\App\\\\app.exe",
  "workingDirectory": "C:\\\\Program Files\\\\App",
  "useMeh": true,
  "useHyperForReverse": false
}
```

#### Invalid Shortcuts:
- Null shortcut
- Invalid key
- Invalid application path
- Invalid working directory
- Empty working directory (valid - optional field)

### Config Validation Tests

#### Valid Config:
```json
{
  "shortcuts": [
    {
      "key": "A",
      "applicationPath": "C:\\\\Program Files\\\\App\\\\app.exe"
    },
    {
      "key": "B",
      "applicationPath": "C:\\\\Program Files\\\\Tool\\\\tool.exe"
    }
  ]
}
```

#### Invalid Configs:
- Null config
- Null shortcuts list
- Empty shortcuts list (no shortcuts configured)
- Too many shortcuts (> 100)
- Duplicate keys: "A" and "A", or "A" and "a" (case-insensitive)
- Any shortcut failing validation (reports shortcut index in error)

### Path Sanitization Tests

#### Sanitization Behavior:
- Expands environment variables: `%TEMP%\\test.exe` → `C:\\Users\\Username\\AppData\\Local\\Temp\\test.exe`
- Normalizes slashes: `C:/Program Files/App/app.exe` → `C:\\Program Files\\App\\app.exe`
- Returns empty for null/empty/whitespace input

## Security Benefits

1. **Injection Attack Prevention**: Blocks directory traversal and command injection attempts
2. **Data Integrity**: Ensures configuration file structure is valid before use
3. **User Error Prevention**: Catches common mistakes in manual JSON editing
4. **Consistent Validation**: Same rules applied at startup and when editing through GUI
5. **Clear Error Messages**: Users get actionable feedback when validation fails

## Manual Testing

To manually test the validation:

1. **Test Invalid JSON Structure**:
   - Delete `shortcuts.json` and create an invalid JSON file
   - Expected: Parse error message on startup

2. **Test Invalid Key**:
   - Edit `shortcuts.json` and change a key to "@@"
   - Expected: Validation error on startup

3. **Test Path Traversal**:
   - Edit `shortcuts.json` and set applicationPath to `"..\\\\..\\\\malicious.exe"`
   - Expected: Validation error on startup

4. **Test Duplicate Keys**:
   - Edit `shortcuts.json` and create two shortcuts with key "A"
   - Expected: Duplicate key error on startup

5. **Test GUI Validation**:
   - Open shortcut manager
   - Try to add shortcut with special character key like "!"
   - Expected: Validation error dialog

6. **Test Environment Variables**:
   - Add shortcut with path `%LOCALAPPDATA%\\Programs\\App\\app.exe`
   - Expected: Path expands and works correctly

## Future Testing Enhancements

**Note**: Both xUnit and NUnit currently have compatibility issues with .NET 10 preview in Visual Studio 2026. The test frameworks package net8.0 libraries which should be forward-compatible, but the .NET 10 preview SDK/tooling has issues resolving test framework types at compile time even with explicit using statements.

The full test suite has been created in `ShortCutTool.Tests/ConfigValidatorTests.cs` with NUnit, covering:
- 6 valid key tests
- 4 invalid key tests (null/empty, multiple characters, special characters)
- 4 valid path tests
- 7 invalid path tests (null/empty, too long, traversal, invalid chars, reserved names, control chars, relative)
- 5 shortcut validation tests
- 8 config validation tests
- 3 path sanitization tests
- 2 ValidationResult tests

Total: **39 automated test cases** covering all validation scenarios.

### Options for Running Tests:

1. **Manual Testing**: Use the manual testing instructions in this document
2. **Wait for .NET 10 RTM**: Test tooling compatibility should improve as .NET 10 approaches release
3. **Target net8.0 for Tests**: Create a separate test project targeting net8.0 that references the main project (though this may have its own compatibility challenges)
4. **Use .NET 8 SDK**: Temporarily switch to .NET 8 SDK for test development, then switch back to .NET 10 for the main application

## Version History

- **v1.0.0**: Initial validation implementation
  - Added ConfigValidator class
  - Integrated validation at startup and in GUI
  - Added comprehensive error messages
  - Created 39 automated test cases (NUnit)
  - Test project created but blocked by .NET 10 preview tooling issues
  - Manual testing recommended until .NET 10 RTM improves test framework compatibility
