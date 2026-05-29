using System.Drawing;
using System.Runtime.InteropServices;

namespace ShortCutTool;

/// <summary>
/// Lightweight popup window that shows cycling windows list.
/// Displays available windows for an application with animated fade in/out effects.
/// Positioned near the system tray and styled to be unobtrusive.
/// </summary>
public class CyclePopup : Form
{
    private readonly List<string> _windowTitles;
    private int _selectedIndex;

    /// <summary>
    /// Animation timing constants.
    /// </summary>
    private const int ANIMATION_DURATION_MS = 150;
    private const int ANIMATION_STEPS = 10;

    /// <summary>
    /// UI styling constants.
    /// </summary>
    private const int TRUNCATE_TITLE_LENGTH = 50;
    private const double MAX_OPACITY = 0.95;

    /// <summary>
    /// Color constants for theming.
    /// </summary>
    private static readonly Color BACKGROUND_COLOR = Color.FromArgb(45, 45, 48);        // Dark background
    private static readonly Color TEXT_SELECTED_COLOR = Color.FromArgb(0, 122, 204);    // Bright blue for selected
    private static readonly Color TEXT_UNSELECTED_COLOR = Color.FromArgb(200, 200, 200); // Light gray for unselected

    /// <summary>
    /// Layout spacing constants.
    /// </summary>
    private const int PANEL_PADDING = 12;
    private const int LABEL_VERTICAL_PADDING = 4;
    private const int SELECTED_FONT_SIZE = 10;
    private const int UNSELECTED_FONT_SIZE = 9;
    private const string FONT_NAME = "Segoe UI";

    private double _opacity = 0;
    private TableLayoutPanel? _layout;

    /// <summary>
    /// Initializes a new instance of the CyclePopup class with the specified window titles and selection.
    /// </summary>
    /// <param name="windowTitles">List of window titles to display</param>
    /// <param name="selectedIndex">Index of the currently selected window (highlighted)</param>
    public CyclePopup(List<string> windowTitles, int selectedIndex)
    {
        _windowTitles = windowTitles;
        _selectedIndex = selectedIndex;

        // Setup form properties for unobtrusive display
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = BACKGROUND_COLOR;
        Opacity = 0;

        SetupUI();
        PositionNearTray();
    }

    private void SetupUI()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(PANEL_PADDING),
            BackColor = BACKGROUND_COLOR
        };

        _layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = _windowTitles.Count,
            BackColor = Color.Transparent
        };

        int rowIndex = 0;
        foreach (var title in _windowTitles)
        {
            var isSelected = rowIndex == _selectedIndex;
            var label = new Label
            {
                Text = $"{(isSelected ? "▶ " : "  ")}{TruncateTitle(title)}",
                AutoSize = true,
                ForeColor = isSelected ? TEXT_SELECTED_COLOR : TEXT_UNSELECTED_COLOR,
                Font = new Font(FONT_NAME, isSelected ? SELECTED_FONT_SIZE : UNSELECTED_FONT_SIZE, isSelected ? FontStyle.Bold : FontStyle.Regular),
                Padding = new Padding(0, LABEL_VERTICAL_PADDING, 0, LABEL_VERTICAL_PADDING),
                BackColor = Color.Transparent
            };

            _layout.Controls.Add(label, 0, rowIndex);
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rowIndex++;
        }

        panel.Controls.Add(_layout);
        Controls.Add(panel);

        // Size the form
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
    }

    private string TruncateTitle(string title, int maxLength = TRUNCATE_TITLE_LENGTH)
    {
        if (title.Length <= maxLength)
            return title;

        return title.Substring(0, maxLength - 3) + "...";
    }

    private void PositionNearTray()
    {
        var taskbarEdge = GetTaskbarEdge();
        Location = CalculatePosition(taskbarEdge);
    }

    /// <summary>
    /// Determines which edge of the screen the taskbar is on.
    /// </summary>
    /// <returns>A string indicating the taskbar position: "Bottom", "Top", "Left", or "Right"</returns>
    private string GetTaskbarEdge()
    {
        APPBARDATA abd = new APPBARDATA();
        abd.cbSize = Marshal.SizeOf(abd);
        SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

        var screen = Screen.PrimaryScreen;
        var screenBounds = screen.Bounds;

        // Determine taskbar edge based on its position
        if (abd.rc.bottom == screenBounds.Height && abd.rc.top > 0)
            return "Bottom"; // Bottom taskbar (most common)

        if (abd.rc.top == 0 && abd.rc.bottom < screenBounds.Height)
            return "Top"; // Top taskbar

        if (abd.rc.left == 0 && abd.rc.right < screenBounds.Width)
            return "Left"; // Left taskbar

        return "Right"; // Right taskbar or default
    }

    /// <summary>
    /// Calculates the optimal position for the popup based on taskbar location.
    /// </summary>
    /// <param name="taskbarEdge">The edge where the taskbar is located</param>
    /// <returns>The calculated point for the popup location</returns>
    private Point CalculatePosition(string taskbarEdge)
    {
        var screen = Screen.PrimaryScreen;
        var workingArea = screen.WorkingArea;
        const int MARGIN = 10;

        return taskbarEdge switch
        {
            "Top" => new Point(workingArea.Right - Width - MARGIN, workingArea.Top + MARGIN),
            "Left" => new Point(workingArea.Left + MARGIN, workingArea.Bottom - Height - MARGIN),
            "Right" => new Point(workingArea.Right - Width - MARGIN, workingArea.Bottom - Height - MARGIN),
            _ => new Point(workingArea.Right - Width - MARGIN, workingArea.Bottom - Height - MARGIN), // Bottom (default)
        };
    }

    /// <summary>
    /// Shows the popup window with fade-in animation.
    /// </summary>
    public void ShowPopup()
    {
        Show();
        FadeIn();
    }

    private async void FadeIn()
    {
        var increment = 1.0 / ANIMATION_STEPS;
        var delay = ANIMATION_DURATION_MS / ANIMATION_STEPS;

        for (int i = 0; i < ANIMATION_STEPS; i++)
        {
            _opacity += increment;
            Opacity = Math.Min(_opacity, MAX_OPACITY);
            await Task.Delay(delay);
        }
    }

    private async void FadeOut()
    {
        var decrement = _opacity / ANIMATION_STEPS;
        var delay = ANIMATION_DURATION_MS / ANIMATION_STEPS;

        for (int i = 0; i < ANIMATION_STEPS; i++)
        {
            _opacity -= decrement;
            Opacity = Math.Max(_opacity, 0);
            await Task.Delay(delay);
        }

        Close();
        Dispose();
    }

    /// <summary>
    /// Updates the popup with a new list of window titles and selected index.
    /// Thread-safe; automatically invokes on UI thread if needed.
    /// </summary>
    /// <param name="windowTitles">Updated list of window titles to display</param>
    /// <param name="selectedIndex">Index of the window that is now selected</param>
    public void UpdateSelection(List<string> windowTitles, int selectedIndex)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => UpdateSelection(windowTitles, selectedIndex)));
            return;
        }

        _selectedIndex = selectedIndex;

        if (_layout == null) return;

        // Update existing labels
        for (int i = 0; i < Math.Min(windowTitles.Count, _layout.Controls.Count); i++)
        {
            if (_layout.Controls[i] is Label label)
            {
                var isSelected = i == selectedIndex;
                label.Text = $"{(isSelected ? "▶ " : "  ")}{TruncateTitle(windowTitles[i])}";
                label.ForeColor = isSelected ? Color.FromArgb(0, 122, 204) : Color.FromArgb(200, 200, 200);
                label.Font = new Font("Segoe UI", isSelected ? 10F : 9F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            }
        }
    }

    /// <summary>
    /// Dismisses the popup with fade-out animation and cleanup.
    /// Thread-safe; automatically invokes on UI thread if needed.
    /// </summary>
    public void DismissPopup()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(DismissPopup));
            return;
        }

        FadeOut();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Remove from Alt+Tab
        int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
        exStyle |= WS_EX_TOOLWINDOW;
        SetWindowLong(Handle, GWL_EXSTYLE, exStyle);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            // Make window click-through and non-activatable
            cp.ExStyle |= WS_EX_TRANSPARENT;
            cp.ExStyle |= WS_EX_NOACTIVATE;
            return cp;
        }
    }

    // P/Invoke for taskbar position
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct APPBARDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    // P/Invoke for taskbar position
    private const int ABM_GETTASKBARPOS = 5;

    [DllImport("shell32.dll")]
    private static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    // P/Invoke window extended style constants
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_NOACTIVATE = 0x08000000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
