using Microsoft.Win32;

namespace ShortCutTool;

/// <summary>
/// Manages the per-user "start with Windows" entry under
/// HKCU\Software\Microsoft\Windows\CurrentVersion\Run. The installer writes and removes
/// the same value, so the tray option and the installer checkbox stay in sync.
/// </summary>
public static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ShortCutTool";

    public static bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            return key?.GetValue(ValueName) is string;
        }
    }

    public static void Enable()
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath);
        key.SetValue(ValueName, Command);
    }

    public static void Disable()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        key?.DeleteValue(ValueName, throwOnMissingValue: false);
    }

    /// <summary>
    /// If the entry exists but points somewhere else (the app was moved or reinstalled
    /// to a new folder), repoint it at the running executable.
    /// </summary>
    public static void RefreshPathIfEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        if (key?.GetValue(ValueName) is string current &&
            !string.Equals(current, Command, StringComparison.OrdinalIgnoreCase))
        {
            key.SetValue(ValueName, Command);
            Log.Info($"Updated startup entry from {current} to {Command}");
        }
    }

    private static string Command => $"\"{Environment.ProcessPath}\"";
}
