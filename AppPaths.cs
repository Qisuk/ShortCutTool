namespace ShortCutTool;

/// <summary>
/// Well-known file locations. Configuration lives in the roaming profile so it survives
/// reinstalls and upgrades; logs live in the local profile.
/// </summary>
public static class AppPaths
{
    private const string AppFolderName = "ShortCutTool";
    private const string ConfigFileName = "shortcuts.json";

    /// <summary>
    /// Gets the folder that holds the user's configuration (%APPDATA%\ShortCutTool).
    /// </summary>
    public static string ConfigDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppFolderName);

    /// <summary>
    /// Gets the full path to the user's shortcuts.json.
    /// </summary>
    public static string ConfigFile { get; } = Path.Combine(ConfigDirectory, ConfigFileName);

    /// <summary>
    /// Gets the folder that holds log files (%LOCALAPPDATA%\ShortCutTool\logs).
    /// </summary>
    public static string LogDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppFolderName, "logs");

    /// <summary>
    /// Gets the location older versions used: shortcuts.json next to the executable.
    /// </summary>
    public static string LegacyConfigFile { get; } = Path.Combine(AppContext.BaseDirectory, ConfigFileName);
}
