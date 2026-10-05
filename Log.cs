using System.Text;

namespace ShortCutTool;

/// <summary>
/// Minimal file logger. Writes to %LOCALAPPDATA%\ShortCutTool\logs\shortcuttool.log and keeps
/// one previous file when the current one exceeds <see cref="MaxFileBytes"/>.
/// Logging must never take the app down, so all I/O failures are swallowed.
/// </summary>
public static class Log
{
    private const long MaxFileBytes = 1024 * 1024;
    private static readonly object Sync = new();
    private static readonly string LogFile = Path.Combine(AppPaths.LogDirectory, "shortcuttool.log");
    private static readonly string PreviousLogFile = Path.Combine(AppPaths.LogDirectory, "shortcuttool.1.log");

    public static void Info(string message) => Write("INFO", message, null);

    public static void Warn(string message, Exception? ex = null) => Write("WARN", message, ex);

    public static void Error(string message, Exception? ex = null) => Write("ERROR", message, ex);

    private static void Write(string level, string message, Exception? ex)
    {
        var line = new StringBuilder()
            .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
            .Append(' ').Append(level).Append(' ').Append(message);
        if (ex != null)
        {
            line.AppendLine().Append(ex);
        }
        line.AppendLine();

        System.Diagnostics.Debug.Write(line.ToString());

        lock (Sync)
        {
            try
            {
                Directory.CreateDirectory(AppPaths.LogDirectory);
                var info = new FileInfo(LogFile);
                if (info.Exists && info.Length > MaxFileBytes)
                {
                    File.Move(LogFile, PreviousLogFile, overwrite: true);
                }
                File.AppendAllText(LogFile, line.ToString());
            }
            catch
            {
                // Ignore - logging is best effort
            }
        }
    }
}
