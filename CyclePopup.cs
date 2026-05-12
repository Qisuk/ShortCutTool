using System.Drawing;
using System.Runtime.InteropServices;

namespace ShortCutTool;

/// <summary>
/// Lightweight popup window that shows cycling windows list
/// </summary>
public class CyclePopup : Form
{
    private readonly List<string> _windowTitles;
    private int _selectedIndex;
    private const int ANIMATION_DURATION_MS = 150;
    private double _opacity = 0;
    private TableLayoutPanel? _layout;

    public CyclePopup(List<string> windowTitles, int selectedIndex)
    {
        _windowTitles = windowTitles;
        _selectedIndex = selectedIndex;

        // Setup form properties for unobtrusive display
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = Color.FromArgb(45, 45, 48);
        Opacity = 0;

        SetupUI();
        PositionNearTray();
    }

    private void SetupUI()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            BackColor = BackColor
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
                ForeColor = isSelected ? Color.FromArgb(0, 122, 204) : Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", isSelected ? 10F : 9F, isSelected ? FontStyle.Bold : FontStyle.Regular),
                Padding = new Padding(0, 4, 0, 4),
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

    private string TruncateTitle(string title, int maxLength = 50)
    {
        if (title.Length <= maxLength)
            return title;

        return title.Substring(0, maxLength - 3) + "...";
    }

    private void PositionNearTray()
    {
        // Get the taskbar info
        APPBARDATA abd = new APPBARDATA();
        abd.cbSize = Marshal.SizeOf(abd);
        SHAppBarMessage(ABM_GETTASKBARPOS, ref abd);

        var screen = Screen.PrimaryScreen;
        var workingArea = screen.WorkingArea;
        var screenBounds = screen.Bounds;

        // Calculate popup position based on taskbar location
        int x, y;
        const int MARGIN = 10;

        // Determine taskbar edge
        if (abd.rc.bottom == screenBounds.Height && abd.rc.top > 0)
        {
            // Bottom taskbar (most common)
            x = workingArea.Right - Width - MARGIN;
            y = workingArea.Bottom - Height - MARGIN;
        }
        else if (abd.rc.top == 0 && abd.rc.bottom < screenBounds.Height)
        {
            // Top taskbar
            x = workingArea.Right - Width - MARGIN;
            y = workingArea.Top + MARGIN;
        }
        else if (abd.rc.left == 0 && abd.rc.right < screenBounds.Width)
        {
            // Left taskbar
            x = workingArea.Left + MARGIN;
            y = workingArea.Bottom - Height - MARGIN;
        }
        else
        {
            // Right taskbar or default to bottom-right
            x = workingArea.Right - Width - MARGIN;
            y = workingArea.Bottom - Height - MARGIN;
        }

        Location = new Point(x, y);
    }

    public void ShowPopup()
    {
        Show();
        FadeIn();
    }

    private async void FadeIn()
    {
        var steps = 10;
        var increment = 1.0 / steps;
        var delay = ANIMATION_DURATION_MS / steps;

        for (int i = 0; i < steps; i++)
        {
            _opacity += increment;
            Opacity = Math.Min(_opacity, 0.95);
            await Task.Delay(delay);
        }
    }

    private async void FadeOut()
    {
        var steps = 10;
        var decrement = _opacity / steps;
        var delay = ANIMATION_DURATION_MS / steps;

        for (int i = 0; i < steps; i++)
        {
            _opacity -= decrement;
            Opacity = Math.Max(_opacity, 0);
            await Task.Delay(delay);
        }

        Close();
        Dispose();
    }

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
            cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
            cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
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

    private const int ABM_GETTASKBARPOS = 5;

    [DllImport("shell32.dll")]
    private static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    // P/Invoke for removing from Alt+Tab
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
