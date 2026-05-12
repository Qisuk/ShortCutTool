using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace ShortCutTool;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly List<ShortcutMapping> _shortcuts;

    public TrayApplicationContext(List<ShortcutMapping> shortcuts)
    {
        _shortcuts = shortcuts;

        _trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            ContextMenuStrip = CreateContextMenu(),
            Visible = true,
            Text = $"ShortCut Tool - {_shortcuts.Count} shortcuts active"
        };

        _trayIcon.DoubleClick += OnTrayIconDoubleClick;
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        var statusItem = new ToolStripMenuItem($"{_shortcuts.Count} shortcuts active")
        {
            Enabled = false
        };
        menu.Items.Add(statusItem);

        menu.Items.Add(new ToolStripSeparator());

        var showShortcutsItem = new ToolStripMenuItem("Show Shortcuts...", null, OnShowShortcuts);
        menu.Items.Add(showShortcutsItem);

        menu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit", null, OnExit);
        menu.Items.Add(exitItem);

        return menu;
    }

    private void OnTrayIconDoubleClick(object? sender, EventArgs e)
    {
        ShowShortcutInformation();
    }

    private void OnShowShortcuts(object? sender, EventArgs e)
    {
        ShowShortcutInformation();
    }

    private void ShowShortcutInformation()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Configured Shortcuts:");
        sb.AppendLine();

        foreach (var shortcut in _shortcuts)
        {
            var modifier = shortcut.UseMeh ? "Ctrl+Alt+Shift" : "None";
            var appName = Path.GetFileNameWithoutExtension(shortcut.ApplicationPath);

            sb.AppendLine($"[{modifier}+{shortcut.Key}]");
            sb.AppendLine($"  Application: {appName}");
            sb.AppendLine($"  Path: {shortcut.ApplicationPath}");

            if (shortcut.UseHyperForReverse)
            {
                sb.AppendLine($"  Reverse: Ctrl+Alt+Shift+Win+{shortcut.Key}");
            }

            if (!string.IsNullOrEmpty(shortcut.WorkingDirectory))
            {
                sb.AppendLine($"  Working Dir: {shortcut.WorkingDirectory}");
            }

            sb.AppendLine();
        }

        sb.AppendLine("Note: Shortcuts will launch the application if not running,");
        sb.AppendLine("or cycle through multiple instances if already running.");
        if (_shortcuts.Any(s => s.UseHyperForReverse))
        {
            sb.AppendLine();
            sb.AppendLine("Hyper key (Ctrl+Alt+Shift+Win) cycles backwards through instances.");
        }

        MessageBox.Show(
            sb.ToString(),
            "ShortCut Tool - Active Shortcuts",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void OnExit(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
