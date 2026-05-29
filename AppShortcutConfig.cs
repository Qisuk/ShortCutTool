namespace ShortCutTool;

/// <summary>
/// Root configuration object that contains the collection of all shortcut mappings.
/// </summary>
public class AppShortcutConfig
{
    /// <summary>
    /// Gets or sets the list of shortcut mappings.
    /// </summary>
    public List<ShortcutMapping> Shortcuts { get; set; } = new();
}

/// <summary>
/// Represents a single keyboard shortcut mapping to an application.
/// </summary>
public class ShortcutMapping
{
    /// <summary>
    /// Gets or sets the single alphanumeric key that triggers this shortcut (A-Z, 0-9).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this shortcut uses the Meh key combination (Ctrl+Alt+Shift).
    /// </summary>
    public bool UseMeh { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Hyper key combination (Ctrl+Alt+Shift+Win) cycles windows in reverse.
    /// </summary>
    public bool UseHyperForReverse { get; set; } = false;

    /// <summary>
    /// Gets or sets the full path to the application executable.
    /// </summary>
    public string ApplicationPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional working directory for the application when launched.
    /// </summary>
    public string? WorkingDirectory { get; set; }
}