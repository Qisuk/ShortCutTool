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
var config = JsonSerializer.Deserialize<AppShortcutConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

if (config == null || config.Shortcuts.Count == 0)
{
    MessageBox.Show(
        "No shortcuts configured. Please edit the configuration file.",
        "ShortCut Tool",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning);
    return;
}

// Expand environment variables in application paths
foreach (var shortcut in config.Shortcuts)
{
    shortcut.ApplicationPath = Environment.ExpandEnvironmentVariables(shortcut.ApplicationPath);
    if (!string.IsNullOrEmpty(shortcut.WorkingDirectory))
    {
        shortcut.WorkingDirectory = Environment.ExpandEnvironmentVariables(shortcut.WorkingDirectory);
    }
}

using var hookService = new KeyboardHookService();

foreach (var shortcut in config.Shortcuts)
{
    hookService.RegisterShortcut(shortcut);
}

var trayApp = new TrayApplicationContext(config.Shortcuts);
Application.Run(trayApp);

