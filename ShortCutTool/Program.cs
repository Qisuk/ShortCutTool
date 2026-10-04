using System.Text.Json;
using ShortCutTool;

namespace ShortCutTool;

class Program
{
    /// <summary>
    /// Name of the single-instance mutex. The installer's AppMutex setting uses the same
    /// name to detect a running copy before upgrading or uninstalling.
    /// </summary>
    public const string SingleInstanceMutexName = "ShortCutTool.SingleInstance";

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        Application.ThreadException += (s, e) => Log.Error("Unhandled UI thread exception", e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Error("Unhandled exception", e.ExceptionObject as Exception);

        using var mutex = new Mutex(initiallyOwned: false, SingleInstanceMutexName);
        if (!TryAcquire(mutex))
        {
            Log.Info("Another instance is already running; exiting");
            MessageBox.Show(
                "ShortCut Tool is already running. Look for its icon in the system tray.",
                "ShortCut Tool",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            Run();
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private static void Run()
    {
        Log.Info($"Starting ShortCut Tool {Application.ProductVersion} from {Environment.ProcessPath}");

        bool isFirstRun;
        AppShortcutConfig? config;

        try
        {
            isFirstRun = ConfigStore.EnsureExists();
            config = ConfigStore.Load();
        }
        catch (JsonException ex)
        {
            Log.Error("Configuration file is not valid JSON", ex);
            MessageBox.Show(
                $"Invalid JSON configuration file:\n{AppPaths.ConfigFile}\n\n{ex.Message}",
                "ShortCut Tool - Configuration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Log.Error("Could not read or create the configuration file", ex);
            MessageBox.Show(
                $"Could not read or create the configuration file:\n{AppPaths.ConfigFile}\n\n{ex.Message}",
                "ShortCut Tool - Configuration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        // Validate configuration
        var validationResult = ConfigValidator.ValidateConfig(config);
        if (!validationResult.IsValid)
        {
            Log.Error($"Configuration validation failed: {validationResult.ErrorMessage}");
            MessageBox.Show(
                $"Configuration validation failed:\n\n{validationResult.ErrorMessage}\n\nPlease fix the configuration file and restart:\n{AppPaths.ConfigFile}",
                "ShortCut Tool - Validation Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        StartupRegistration.RefreshPathIfEnabled();

        using var hookService = new KeyboardHookService();

        // Register expanded copies so the saved configuration keeps its environment
        // variables (e.g. %USERNAME%) and stays portable between machines.
        foreach (var shortcut in config!.Shortcuts)
        {
            hookService.RegisterShortcut(ExpandPaths(shortcut));
        }

        Log.Info($"Registered {config.Shortcuts.Count} shortcut(s)");

        var openManagerOnStart = isFirstRun || config.Shortcuts.Count == 0;
        var trayApp = new TrayApplicationContext(config.Shortcuts, openManagerOnStart);
        Application.Run(trayApp);

        Log.Info("Exiting");
    }

    /// <summary>
    /// Waits briefly for the mutex so that "Save &amp; Restart" works: the new process starts
    /// while the old one is still shutting down.
    /// </summary>
    private static bool TryAcquire(Mutex mutex)
    {
        try
        {
            return mutex.WaitOne(TimeSpan.FromSeconds(5));
        }
        catch (AbandonedMutexException)
        {
            // The previous owner exited without releasing it; we own it now.
            return true;
        }
    }

    private static ShortcutMapping ExpandPaths(ShortcutMapping shortcut) => new()
    {
        Key = shortcut.Key,
        UseMeh = shortcut.UseMeh,
        UseHyperForReverse = shortcut.UseHyperForReverse,
        ApplicationPath = ConfigValidator.SanitizePath(shortcut.ApplicationPath),
        WorkingDirectory = string.IsNullOrEmpty(shortcut.WorkingDirectory)
            ? shortcut.WorkingDirectory
            : ConfigValidator.SanitizePath(shortcut.WorkingDirectory)
    };
}
