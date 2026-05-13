using System.Text.Json;
using ShortCutTool;

ApplicationConfiguration.Initialize();

const string configFile = "shortcuts.json";

if (!File.Exists(configFile))
{
    MessageBox.Show(
        $"Configuration file '{configFile}' not found. Creating default configuration file.",
        "ShortCut Tool",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);

    var defaultConfig = new AppShortcutConfig
    {
        Shortcuts = new List<ShortcutMapping>
        {
            new()
            {
                Key = "C",
                UseMeh = true,
                ApplicationPath = "C:\\Users\\YourUsername\\AppData\\Local\\Programs\\Microsoft VS Code\\Code.exe"
            },
            new()
            {
                Key = "V",
                UseMeh = true,
                ApplicationPath = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe"
            }
        }
    };

    var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(configFile, json);
}

var configJson = File.ReadAllText(configFile);
AppShortcutConfig? config;

try
{
    config = JsonSerializer.Deserialize<AppShortcutConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
}
catch (JsonException ex)
{
    MessageBox.Show(
        $"Invalid JSON configuration file:\n\n{ex.Message}",
        "ShortCut Tool - Configuration Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    return;
}

// Validate configuration
var validationResult = ConfigValidator.ValidateConfig(config);
if (!validationResult.IsValid)
{
    MessageBox.Show(
        $"Configuration validation failed:\n\n{validationResult.ErrorMessage}\n\nPlease fix the configuration file and restart.",
        "ShortCut Tool - Validation Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    return;
}

if (config == null || config.Shortcuts.Count == 0)
{
    MessageBox.Show(
        "No shortcuts configured. Please edit the configuration file.",
        "ShortCut Tool",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning);
    return;
}

// Sanitize and expand environment variables in application paths
foreach (var shortcut in config.Shortcuts)
{
    shortcut.ApplicationPath = ConfigValidator.SanitizePath(shortcut.ApplicationPath);
    if (!string.IsNullOrEmpty(shortcut.WorkingDirectory))
    {
        shortcut.WorkingDirectory = ConfigValidator.SanitizePath(shortcut.WorkingDirectory);
    }
}

using var hookService = new KeyboardHookService();

foreach (var shortcut in config.Shortcuts)
{
    hookService.RegisterShortcut(shortcut);
}

var trayApp = new TrayApplicationContext(config.Shortcuts);
Application.Run(trayApp);

