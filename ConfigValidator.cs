using System.Text.RegularExpressions;

namespace ShortCutTool;

/// <summary>
/// Validates configuration to prevent injection attacks and ensure data integrity
/// </summary>
public static class ConfigValidator
{
    private const int MaxPathLength = 500;
    private const int MaxKeyLength = 1;
    private const int MaxShortcutsCount = 100;

    /// <summary>
    /// Validates the entire configuration
    /// </summary>
    public static ValidationResult ValidateConfig(AppShortcutConfig? config)
    {
        if (config == null)
        {
            return ValidationResult.Failure("Configuration cannot be null");
        }

        if (config.Shortcuts == null)
        {
            return ValidationResult.Failure("Shortcuts list cannot be null");
        }

        if (config.Shortcuts.Count > MaxShortcutsCount)
        {
            return ValidationResult.Failure($"Too many shortcuts (max: {MaxShortcutsCount})");
        }

        // Check for duplicate keys
        var duplicateKeys = config.Shortcuts
            .GroupBy(s => s.Key.ToUpperInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateKeys.Any())
        {
            return ValidationResult.Failure($"Duplicate keys found: {string.Join(", ", duplicateKeys)}");
        }

        // Validate each shortcut
        for (int i = 0; i < config.Shortcuts.Count; i++)
        {
            var result = ValidateShortcut(config.Shortcuts[i]);
            if (!result.IsValid)
            {
                return ValidationResult.Failure($"Shortcut {i + 1}: {result.ErrorMessage}");
            }
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates a single shortcut mapping
    /// </summary>
    public static ValidationResult ValidateShortcut(ShortcutMapping? shortcut)
    {
        if (shortcut == null)
        {
            return ValidationResult.Failure("Shortcut cannot be null");
        }

        // Validate Key
        var keyResult = ValidateKey(shortcut.Key);
        if (!keyResult.IsValid)
        {
            return keyResult;
        }

        // Validate ApplicationPath
        var pathResult = ValidatePath(shortcut.ApplicationPath, "Application path");
        if (!pathResult.IsValid)
        {
            return pathResult;
        }

        // Validate WorkingDirectory (optional)
        if (!string.IsNullOrEmpty(shortcut.WorkingDirectory))
        {
            var workDirResult = ValidateWorkingDirectory(shortcut.WorkingDirectory);
            if (!workDirResult.IsValid)
            {
                return workDirResult;
            }
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates a key (must be a single alphanumeric character)
    /// </summary>
    public static ValidationResult ValidateKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return ValidationResult.Failure("Key cannot be empty");
        }

        if (key.Length != MaxKeyLength)
        {
            return ValidationResult.Failure("Key must be a single character");
        }

        // Only allow alphanumeric characters
        if (!Regex.IsMatch(key, @"^[a-zA-Z0-9]$"))
        {
            return ValidationResult.Failure("Key must be an alphanumeric character (A-Z, 0-9)");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates an application path
    /// </summary>
    public static ValidationResult ValidatePath(string? path, string fieldName = "Path")
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return ValidationResult.Failure($"{fieldName} cannot be empty");
        }

        if (path.Length > MaxPathLength)
        {
            return ValidationResult.Failure($"{fieldName} is too long (max: {MaxPathLength} characters)");
        }

        // Check for path injection attempts
        var dangerousPatterns = new[]
        {
            @"\.\.",           // Directory traversal
            @"^/",             // Unix absolute path
            @"^\\\\",          // UNC path (could be network injection)
            @"[<>|""]",        // Invalid Windows filename characters
            @"[\x00-\x1F]",    // Control characters
            @"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])($|\.)", // Reserved Windows device names
        };

        foreach (var pattern in dangerousPatterns)
        {
            if (Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase))
            {
                return ValidationResult.Failure($"{fieldName} contains invalid characters or patterns");
            }
        }

        // Ensure path looks like a valid Windows path
        try
        {
            var fullPath = Path.GetFullPath(path);

            // Check if path is absolute
            if (!Path.IsPathFullyQualified(path) && !path.Contains("%"))
            {
                return ValidationResult.Failure($"{fieldName} must be an absolute path or contain environment variables");
            }
        }
        catch (Exception ex)
        {
            return ValidationResult.Failure($"{fieldName} is not a valid path: {ex.Message}");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates a working directory path
    /// </summary>
    public static ValidationResult ValidateWorkingDirectory(string? workingDirectory)
    {
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            return ValidationResult.Success(); // Optional field
        }

        return ValidatePath(workingDirectory, "Working directory");
    }

    /// <summary>
    /// Sanitizes a path by expanding environment variables and normalizing
    /// </summary>
    public static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        try
        {
            // Expand environment variables
            var expanded = Environment.ExpandEnvironmentVariables(path);

            // Normalize path separators
            expanded = expanded.Replace('/', '\\');

            // Get full path to resolve any relative components
            return Path.GetFullPath(expanded);
        }
        catch
        {
            return path; // Return original if sanitization fails
        }
    }
}

/// <summary>
/// Result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new ValidationResult(true);
    public static ValidationResult Failure(string errorMessage) => new ValidationResult(false, errorMessage);
}
