using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ShortCutTool;

/// <summary>
/// Provides utilities to enumerate all visible windows for a given process.
/// This addresses the limitation of Process.MainWindowHandle which only returns the first window per process.
/// Useful for applications like VS Code that spawn multiple windows under the same process.
/// </summary>
public static class WindowEnumerator
{
    /// <summary>
    /// Callback delegate for EnumWindows API.
    /// Return true to continue enumeration, false to stop.
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="lParam">Application-defined value (unused)</param>
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    /// <summary>
    /// Enumerates all top-level windows on the screen.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window
    /// and, optionally, the identifier of the process that created the window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    /// <summary>
    /// Determines the visibility state of the specified window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    /// <summary>
    /// Copies the text of the specified window's title bar into a buffer.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    /// <summary>
    /// Retrieves the length, in characters, of the text of the specified window's title bar.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    /// <summary>
    /// Gets all visible windows for the specified process name.
    /// </summary>
    /// <param name="processName">The name of the process (without .exe extension)</param>
    /// <returns>List of window information for visible windows owned by the process</returns>
    public static List<WindowInfo> GetProcessWindows(string processName)
    {
        var windows = new List<WindowInfo>();
        var processes = Process.GetProcessesByName(processName);
        var processIds = new HashSet<uint>(processes.Select(p => (uint)p.Id));

        EnumWindows((hWnd, lParam) =>
        {
            // Skip hidden windows
            if (!IsWindowVisible(hWnd))
                return true;

            // Get the process ID for this window
            GetWindowThreadProcessId(hWnd, out uint processId);

            // Skip windows not owned by our target process
            if (!processIds.Contains(processId))
                return true;

            // Get the window title length
            int titleLength = GetWindowTextLength(hWnd);
            if (titleLength == 0)
                return true; // Skip windows with no title

            // Retrieve the window title
            var titleBuilder = new StringBuilder(titleLength + 1);
            GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
            string title = titleBuilder.ToString();

            // Skip windows with empty or whitespace-only titles
            if (!string.IsNullOrWhiteSpace(title))
            {
                windows.Add(new WindowInfo
                {
                    ProcessId = (int)processId,
                    WindowHandle = hWnd,
                    Title = title
                });
            }

            return true; // Continue enumeration
        }, IntPtr.Zero);

        return windows;
    }
}

/// <summary>
/// Represents information about an enumerated window.
/// </summary>
public class WindowInfo
{
    /// <summary>
    /// Gets or sets the process ID that owns this window.
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// Gets or sets the window handle (HWND).
    /// </summary>
    public IntPtr WindowHandle { get; set; }

    /// <summary>
    /// Gets or sets the window title text.
    /// </summary>
    public string Title { get; set; } = string.Empty;
}
