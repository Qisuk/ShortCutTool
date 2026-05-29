using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShortCutTool;

public class KeyboardHookService : IDisposable
{
    /// <summary>
    /// Windows hook constants.
    /// </summary>
    private const int WH_KEYBOARD_LL = 13;           // Low-level keyboard hook.
    private const int WM_KEYDOWN = 0x0100;           // Key pressed.
    private const int WM_SYSKEYDOWN = 0x0104;        // System key (Alt) pressed.
    private const int WM_KEYUP = 0x0101;             // Key released.
    private const int WM_SYSKEYUP = 0x0105;          // System key (Alt) released.

    private readonly LowLevelKeyboardProc _proc;
    private readonly IntPtr _hookId = IntPtr.Zero;
    private readonly Dictionary<string, ShortcutMapping> _shortcuts = new();
    private readonly Dictionary<string, int> _currentInstanceIndex = new();
    private readonly ModifierKeyState _modifierKeyState = new();

    private CyclePopup? _activePopup;

    /// <summary>
    /// Initializes a new instance of the KeyboardHookService class.
    /// Sets up the low-level keyboard hook for global hotkey capture.
    /// </summary>
    public KeyboardHookService()
    {
        _proc = HookCallback;
        _hookId = SetHook(_proc);
    }

    /// <summary>
    /// Registers a new keyboard shortcut that will be monitored by the hook.
    /// </summary>
    /// <param name="mapping">The shortcut mapping configuration to register</param>
    public void RegisterShortcut(ShortcutMapping mapping)
    {
        var key = mapping.Key.ToUpperInvariant();
        _shortcuts[key] = mapping;
    }

    /// <summary>
    /// Disposes the keyboard hook and releases resources.
    /// Should be called when the service is no longer needed to uninstall the global hook.
    /// </summary>
    public void Dispose()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule?.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var key = ((Keys)vkCode).ToString().ToUpperInvariant();

            // Update modifier key state
            _modifierKeyState.HandleKeyDown(key);

            // Check for shortcut activation
            if (_shortcuts.TryGetValue(key, out var mapping))
            {
                if (mapping.UseMeh && _modifierKeyState.IsMehPressed)
                {
                    Task.Run(() => HandleShortcut(mapping, key, false));
                    return (IntPtr)1; // Suppress the key
                }
                else if (mapping.UseHyperForReverse && _modifierKeyState.IsHyperPressed)
                {
                    Task.Run(() => HandleShortcut(mapping, key, true));
                    return (IntPtr)1; // Suppress the key
                }
            }
        }
        else if (nCode >= 0 && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var key = ((Keys)vkCode).ToString().ToUpperInvariant();

            // Update modifier key state
            _modifierKeyState.HandleKeyUp(key);

            // Dismiss popup when all modifiers are released
            if (!_modifierKeyState.AnyModifierPressed && _activePopup != null)
            {
                var popup = _activePopup;
                _activePopup = null;
                Task.Run(() =>
                {
                    try
                    {
                        popup.DismissPopup();
                    }
                    catch { }
                });
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    /// <summary>
    /// Handles the execution of a registered shortcut by launching or cycling windows.
    /// Attempts to find running instances of the application, or launches it if not running.
    /// For multiple instances, displays an async popup and cycles through them.
    /// </summary>
    /// <param name="mapping">The shortcut mapping configuration</param>
    /// <param name="key">The key that was pressed (for tracking current instance)</param>
    /// <param name="reverse">Whether to cycle backward through instances</param>
    private void HandleShortcut(ShortcutMapping mapping, string key, bool reverse)
    {
        try
        {
            var appPath = mapping.ApplicationPath;
            var processName = Path.GetFileNameWithoutExtension(appPath);

            // Use enhanced window enumeration to find ALL windows
            var windows = WindowEnumerator.GetProcessWindows(processName);

            if (windows.Count == 0)
            {
                // No instances running, launch the application
                var startInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    UseShellExecute = true
                };

                if (!string.IsNullOrEmpty(mapping.WorkingDirectory))
                {
                    startInfo.WorkingDirectory = mapping.WorkingDirectory;
                }

                Process.Start(startInfo);
                _currentInstanceIndex[key] = 0;
            }
            else if (windows.Count == 1)
            {
                // Only one instance, just bring it to front
                BringWindowToFront(windows[0].WindowHandle);
                _currentInstanceIndex[key] = 0;
            }
            else
            {
                // Multiple instances, cycle through them
                if (!_currentInstanceIndex.TryGetValue(key, out var currentIndex))
                {
                    currentIndex = -1;
                }

                // Cycle forward or backward
                if (reverse)
                {
                    currentIndex--;
                    if (currentIndex < 0)
                    {
                        currentIndex = windows.Count - 1;
                    }
                }
                else
                {
                    currentIndex++;
                    if (currentIndex >= windows.Count)
                    {
                        currentIndex = 0;
                    }
                }

                _currentInstanceIndex[key] = currentIndex;

                var targetWindow = windows[currentIndex];
                BringWindowToFront(targetWindow.WindowHandle);

                // Show popup with window list (non-blocking, async)
                Task.Run(() =>
                {
                    try
                    {
                        var windowTitles = windows.Select(w => w.Title).ToList();

                        // Dismiss existing popup if any
                        if (_activePopup != null)
                        {
                            _activePopup.UpdateSelection(windowTitles, currentIndex);
                        }
                        else
                        {
                            // Create and show popup on UI thread
                            _activePopup = new CyclePopup(windowTitles, currentIndex);
                            _activePopup.ShowPopup();
                        }
                    }
                    catch (Exception popupEx)
                    {
                        // Log popup errors but don't interrupt cycling - popup is non-critical UI feedback
                        System.Diagnostics.Debug.WriteLine($"Warning: Failed to display cycle popup: {popupEx.Message}");
                    }
                });
            }
        }
        catch (Exception ex)
        {
            // Log and display errors in shortcut handling
            System.Diagnostics.Debug.WriteLine($"Error handling shortcut '{key}': {ex.Message}\n{ex.StackTrace}");
            MessageBox.Show($"Error handling shortcut: {ex.Message}\n\n{ex.StackTrace}", "ShortCutTool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Brings the specified window to the foreground and restores it if minimized.
    /// Uses multiple methods for reliability across different window states.
    /// Handles thread attachment for cross-thread window operations.
    /// </summary>
    /// <param name="windowHandle">The handle of the window to bring to front</param>
    private void BringWindowToFront(IntPtr windowHandle)
    {
        // Restore if minimized
        if (IsIconic(windowHandle))
        {
            ShowWindow(windowHandle, SW_RESTORE);
        }

        // Get current foreground window
        IntPtr currentForeground = GetForegroundWindow();

        // Get the thread IDs
        uint currentThreadId = GetCurrentThreadId();
        uint foregroundThreadId = GetWindowThreadProcessId(currentForeground, IntPtr.Zero);
        uint targetThreadId = GetWindowThreadProcessId(windowHandle, IntPtr.Zero);

        // Attach to the foreground thread to allow setting foreground window
        if (foregroundThreadId != currentThreadId)
        {
            AttachThreadInput(currentThreadId, foregroundThreadId, true);
            AttachThreadInput(currentThreadId, targetThreadId, true);
        }

        // Bring window to front using multiple methods for reliability
        BringWindowToTop(windowHandle);
        ShowWindow(windowHandle, SW_SHOW);
        SetForegroundWindow(windowHandle);
        SetFocus(windowHandle);

        // Flash the window to get user attention if needed
        var flashInfo = new FLASHWINFO
        {
            cbSize = (uint)Marshal.SizeOf<FLASHWINFO>(),
            hwnd = windowHandle,
            dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG,
            uCount = 3,
            dwTimeout = 0
        };
        FlashWindowEx(ref flashInfo);

        // Detach thread input
        if (foregroundThreadId != currentThreadId)
        {
            AttachThreadInput(currentThreadId, foregroundThreadId, false);
            AttachThreadInput(currentThreadId, targetThreadId, false);
        }
    }

    /// <summary>
    /// Callback delegate for low-level keyboard events.
    /// </summary>
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Installs a hook procedure into a hook chain.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// Removes a hook procedure from the hook chain.
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Retrieves a module handle for the specified module.
    /// </summary>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    /// <summary>
    /// Sets the foreground window (the window receives keyboard input).
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Brings the specified window to the top of the Z-order.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    /// <summary>
    /// Sets the specified window's show state.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>
    /// Determines whether the specified window is minimized (iconic).
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    /// <summary>
    /// Retrieves a handle to the foreground window (the window which the user is currently working with).
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    /// <summary>
    /// Retrieves the current thread identifier.
    /// </summary>
    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    /// <summary>
    /// Attaches or detaches the input processing mechanism of one thread to that of another thread.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    /// <summary>
    /// Sets the keyboard focus to the specified window.
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

    /// <summary>
    /// Contains the flash status for a window and the area of the window to be flashed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct FLASHWINFO
    {
        /// <summary>The size of the structure, in bytes.</summary>
        public uint cbSize;

        /// <summary>A handle to the window to be flashed.</summary>
        public IntPtr hwnd;

        /// <summary>The flash status.</summary>
        public uint dwFlags;

        /// <summary>The number of times to flash the window.</summary>
        public uint uCount;

        /// <summary>The rate at which the window is to be flashed, in milliseconds.</summary>
        public uint dwTimeout;
    }

    /// <summary>
    /// Window show command constants.
    /// </summary>
    private const int SW_RESTORE = 9;  // Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position.
    private const int SW_SHOW = 5;     // Activates the window and displays it in its current size and position.

    /// <summary>
    /// Flash window flags.
    /// </summary>
    private const uint FLASHW_TRAY = 0x00000002;        // Flash the window's title bar.
    private const uint FLASHW_TIMERNOFG = 0x0000000C;   // Flash continuously until the window comes to the foreground.

    private enum Keys
    {
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = 3,
        MButton = 4,
        XButton1 = 5,
        XButton2 = 6,
        Back = 8,
        Tab = 9,
        LineFeed = 10,
        Clear = 12,
        Return = 13,
        Enter = 13,
        ShiftKey = 16,
        ControlKey = 17,
        Menu = 18,
        Pause = 19,
        Capital = 20,
        CapsLock = 20,
        KanaMode = 21,
        HanguelMode = 21,
        HangulMode = 21,
        JunjaMode = 23,
        FinalMode = 24,
        HanjaMode = 25,
        KanjiMode = 25,
        Escape = 27,
        IMEConvert = 28,
        IMENonconvert = 29,
        IMEAccept = 30,
        IMEAceept = 30,
        IMEModeChange = 31,
        Space = 32,
        Prior = 33,
        PageUp = 33,
        Next = 34,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        Snapshot = 44,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LShiftKey = 160,
        RShiftKey = 161,
        LControlKey = 162,
        RControlKey = 163,
        LMenu = 164,
        RMenu = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        Oem1 = 186,
        Oemplus = 187,
        Oemcomma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        Oem2 = 191,
        Oemtilde = 192,
        Oem3 = 192,
        OemOpenBrackets = 219,
        Oem4 = 219,
        OemPipe = 220,
        Oem5 = 220,
        OemCloseBrackets = 221,
        Oem6 = 221,
        OemQuotes = 222,
        Oem7 = 222,
        Oem8 = 223,
        OemBackslash = 226,
        Oem102 = 226,
        ProcessKey = 229,
        Packet = 231,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        NoName = 252,
        Pa1 = 253,
        OemClear = 254
    }
}
