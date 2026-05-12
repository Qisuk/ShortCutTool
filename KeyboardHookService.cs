using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ShortCutTool;

public class KeyboardHookService : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYUP = 0x0105;

    private readonly LowLevelKeyboardProc _proc;
    private readonly IntPtr _hookId = IntPtr.Zero;
    private readonly Dictionary<string, ShortcutMapping> _shortcuts = new();
    private readonly Dictionary<string, int> _currentInstanceIndex = new();

    private bool _ctrlPressed;
    private bool _altPressed;
    private bool _shiftPressed;
    private bool _winPressed;

    public KeyboardHookService()
    {
        _proc = HookCallback;
        _hookId = SetHook(_proc);
    }

    public void RegisterShortcut(ShortcutMapping mapping)
    {
        var key = mapping.Key.ToUpperInvariant();
        _shortcuts[key] = mapping;
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

            // Track modifier keys
            if (key == "LCONTROLKEY" || key == "RCONTROLKEY" || key == "CONTROLKEY")
            {
                _ctrlPressed = true;
            }
            else if (key == "LMENU" || key == "RMENU" || key == "MENU")
            {
                _altPressed = true;
            }
            else if (key == "LSHIFTKEY" || key == "RSHIFTKEY" || key == "SHIFTKEY")
            {
                _shiftPressed = true;
            }
            else if (key == "LWIN" || key == "RWIN")
            {
                _winPressed = true;
            }
            else
            {
                // Check for shortcut activation
                if (_shortcuts.TryGetValue(key, out var mapping))
                {
                    bool mehPressed = _ctrlPressed && _altPressed && _shiftPressed && !_winPressed;
                    bool hyperPressed = _ctrlPressed && _altPressed && _shiftPressed && _winPressed;

                    if (mapping.UseMeh && mehPressed)
                    {
                        Task.Run(() => HandleShortcut(mapping, key, false));
                        return (IntPtr)1; // Suppress the key
                    }
                    else if (mapping.UseHyperForReverse && hyperPressed)
                    {
                        Task.Run(() => HandleShortcut(mapping, key, true));
                        return (IntPtr)1; // Suppress the key
                    }
                }
            }
        }
        else if (nCode >= 0 && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var key = ((Keys)vkCode).ToString().ToUpperInvariant();

            // Release modifier keys
            if (key == "LCONTROLKEY" || key == "RCONTROLKEY" || key == "CONTROLKEY")
            {
                _ctrlPressed = false;
            }
            else if (key == "LMENU" || key == "RMENU" || key == "MENU")
            {
                _altPressed = false;
            }
            else if (key == "LSHIFTKEY" || key == "RSHIFTKEY" || key == "SHIFTKEY")
            {
                _shiftPressed = false;
            }
            else if (key == "LWIN" || key == "RWIN")
            {
                _winPressed = false;
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void HandleShortcut(ShortcutMapping mapping, string key, bool reverse)
    {
        try
        {
            var appPath = mapping.ApplicationPath;
            var processName = Path.GetFileNameWithoutExtension(appPath);

            var runningProcesses = Process.GetProcessesByName(processName)
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .ToList();

            if (runningProcesses.Count == 0)
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
            else if (runningProcesses.Count == 1)
            {
                // Only one instance, just bring it to front
                BringWindowToFront(runningProcesses[0].MainWindowHandle);
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
                        currentIndex = runningProcesses.Count - 1;
                    }
                }
                else
                {
                    currentIndex++;
                    if (currentIndex >= runningProcesses.Count)
                    {
                        currentIndex = 0;
                    }
                }

                _currentInstanceIndex[key] = currentIndex;

                var targetProcess = runningProcesses[currentIndex];
                BringWindowToFront(targetProcess.MainWindowHandle);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error handling shortcut: {ex.Message}", "ShortCutTool Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BringWindowToFront(IntPtr windowHandle)
    {
        if (IsIconic(windowHandle))
        {
            ShowWindow(windowHandle, SW_RESTORE);
        }

        SetForegroundWindow(windowHandle);
        BringWindowToTop(windowHandle);
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    private const int SW_RESTORE = 9;

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
