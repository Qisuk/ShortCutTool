using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ShortCutTool;

/// <summary>
/// Helper class to find all windows for a given process, including those without MainWindowHandle
/// </summary>
public static class WindowEnumerator
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    public static List<WindowInfo> GetProcessWindows(string processName)
    {
        var windows = new List<WindowInfo>();
        var processes = Process.GetProcessesByName(processName);
        var processIds = new HashSet<uint>(processes.Select(p => (uint)p.Id));

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            GetWindowThreadProcessId(hWnd, out uint processId);

            if (!processIds.Contains(processId))
                return true;

            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return true;

            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            string title = sb.ToString();

            if (!string.IsNullOrWhiteSpace(title))
            {
                windows.Add(new WindowInfo
                {
                    ProcessId = (int)processId,
                    WindowHandle = hWnd,
                    Title = title
                });
            }

            return true;
        }, IntPtr.Zero);

        return windows;
    }
}

public class WindowInfo
{
    public int ProcessId { get; set; }
    public IntPtr WindowHandle { get; set; }
    public string Title { get; set; } = string.Empty;
}