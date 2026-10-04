using System.Text.Json;

namespace ShortCutTool;

/// <summary>
/// Loads and saves the user's configuration at <see cref="AppPaths.ConfigFile"/>.
/// </summary>
public static class ConfigStore
{
    private static readonly JsonSerializerOptions ReadOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    /// <summary>
    /// Ensures a configuration file exists, migrating one from next to the executable
    /// (where older versions kept it) or creating an empty one.
    /// </summary>
    /// <returns>True when a brand new, empty configuration was created.</returns>
    public static bool EnsureExists()
    {
        if (File.Exists(AppPaths.ConfigFile))
        {
            return false;
        }

        Directory.CreateDirectory(AppPaths.ConfigDirectory);

        if (File.Exists(AppPaths.LegacyConfigFile))
        {
            File.Copy(AppPaths.LegacyConfigFile, AppPaths.ConfigFile);
            Log.Info($"Migrated configuration from {AppPaths.LegacyConfigFile} to {AppPaths.ConfigFile}");
            return false;
        }

        Save(new AppShortcutConfig());
        Log.Info($"Created empty configuration at {AppPaths.ConfigFile}");
        return true;
    }

    /// <summary>
    /// Reads the configuration file. Throws <see cref="JsonException"/> if it is malformed.
    /// </summary>
    public static AppShortcutConfig? Load()
    {
        var json = File.ReadAllText(AppPaths.ConfigFile);
        return JsonSerializer.Deserialize<AppShortcutConfig>(json, ReadOptions);
    }

    /// <summary>
    /// Writes the configuration file, replacing it atomically so a crash mid-write
    /// cannot leave a truncated file behind.
    /// </summary>
    public static void Save(AppShortcutConfig config)
    {
        Directory.CreateDirectory(AppPaths.ConfigDirectory);
        var tempFile = AppPaths.ConfigFile + ".tmp";
        File.WriteAllText(tempFile, JsonSerializer.Serialize(config, WriteOptions));
        File.Move(tempFile, AppPaths.ConfigFile, overwrite: true);
    }
}
