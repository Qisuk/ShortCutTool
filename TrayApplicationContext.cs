using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;

namespace ShortCutTool;

/// <summary>
/// Manages the system tray application context, including the tray icon and context menu.
/// Provides UI for viewing and managing shortcuts.
/// </summary>
public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly List<ShortcutMapping> _shortcuts;

    /// <summary>
    /// Initializes a new instance of the TrayApplicationContext class.
    /// </summary>
    /// <param name="shortcuts">The list of configured shortcuts to display</param>
    /// <param name="openManagerOnStart">Open the Shortcut Manager once the message loop starts (first run / nothing configured)</param>
    public TrayApplicationContext(List<ShortcutMapping> shortcuts, bool openManagerOnStart = false)
    {
        _shortcuts = shortcuts;

        _trayIcon = new NotifyIcon
        {
            Icon = LoadApplicationIcon(),
            ContextMenuStrip = CreateContextMenu(),
            Visible = true,
            Text = $"ShortCut Tool - {_shortcuts.Count} shortcuts active"
        };

        _trayIcon.DoubleClick += OnTrayIconDoubleClick;

        // Exit cleanly on sign-out, and when an installer asks running apps to close
        // (Restart Manager sends the same session-end messages).
        SystemEvents.SessionEnded += OnSessionEnded;

        if (openManagerOnStart)
        {
            // Defer until Application.Run has started the message loop.
            var timer = new System.Windows.Forms.Timer { Interval = 1 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                ShowShortcutInformation();
            };
            timer.Start();
        }
    }

    private Icon LoadApplicationIcon()
    {
        // Try to load the custom icon from the application
        try
        {
            // The icon is embedded in the .exe by the build process
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShortCutTool.exe");
            if (File.Exists(iconPath))
            {
                var icon = Icon.ExtractAssociatedIcon(iconPath);
                if (icon != null)
                    return icon;
            }
        }
        catch
        {
            // Fall through to default icon
        }

        // Fallback: try to load from the icon file directly (for development/debugging)
        try
        {
            var iconFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Icons", "app.ico");
            if (File.Exists(iconFilePath))
            {
                return new Icon(iconFilePath, 16, 16); // Use 16x16 for tray
            }
        }
        catch
        {
            // Fall through to system icon
        }

        // Final fallback: use system application icon
        return SystemIcons.Application;
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

        var startWithWindowsItem = new ToolStripMenuItem("Start with Windows", null, OnToggleStartWithWindows)
        {
            Checked = StartupRegistration.IsEnabled
        };
        menu.Items.Add(startWithWindowsItem);

        var openConfigFolderItem = new ToolStripMenuItem("Open Config Folder", null, (s, e) => OpenFolder(AppPaths.ConfigDirectory));
        menu.Items.Add(openConfigFolderItem);

        var openLogFolderItem = new ToolStripMenuItem("Open Log Folder", null, (s, e) => OpenFolder(AppPaths.LogDirectory));
        menu.Items.Add(openLogFolderItem);

        var aboutItem = new ToolStripMenuItem("About...", null, OnAbout);
        menu.Items.Add(aboutItem);

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

    private void OnToggleStartWithWindows(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem item) return;

        try
        {
            if (item.Checked)
            {
                StartupRegistration.Disable();
            }
            else
            {
                StartupRegistration.Enable();
            }
            item.Checked = StartupRegistration.IsEnabled;
            Log.Info($"Start with Windows set to {item.Checked}");
        }
        catch (Exception ex)
        {
            Log.Error("Could not change the Start with Windows setting", ex);
            MessageBox.Show($"Could not change the startup setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void OpenFolder(string folder)
    {
        try
        {
            Directory.CreateDirectory(folder);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Log.Error($"Could not open folder {folder}", ex);
            MessageBox.Show($"Could not open {folder}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnAbout(object? sender, EventArgs e)
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = $"{version?.Major}.{version?.Minor}.{version?.Build}";

        var aboutMessage = new StringBuilder();
        aboutMessage.AppendLine("ShortCut Tool");
        aboutMessage.AppendLine($"Version {versionString}");
        aboutMessage.AppendLine();
        aboutMessage.AppendLine("A keyboard shortcut manager using Meh and Hyper keys");
        aboutMessage.AppendLine("to launch and cycle through application windows.");
        aboutMessage.AppendLine();
        aboutMessage.AppendLine("Meh = Ctrl+Alt+Shift");
        aboutMessage.AppendLine("Hyper = Ctrl+Alt+Shift+Win");
        aboutMessage.AppendLine();
        aboutMessage.AppendLine("© 2026");
        aboutMessage.AppendLine();
        aboutMessage.AppendLine("https://github.com/Qisuk/ShortCutTool");

        MessageBox.Show(
            aboutMessage.ToString(),
            "About ShortCut Tool",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ShowShortcutInformation()
    {
        using var form = new Form
        {
            Text = "Shortcut Manager",
            Size = new Size(900, 600),
            StartPosition = FormStartPosition.CenterScreen,
            MinimizeBox = false,
            MaximizeBox = true,
            FormBorderStyle = FormBorderStyle.Sizable
        };

        // Create DataGridView for shortcuts
        var dataGridView = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        // Define columns
        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Key",
            DataPropertyName = "Key",
            Width = 50,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Application",
            DataPropertyName = "ApplicationName",
            Width = 150,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });

        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Path",
            DataPropertyName = "ApplicationPath",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "Meh",
            DataPropertyName = "UseMeh",
            Width = 50,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });

        dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
        {
            HeaderText = "Hyper Reverse",
            DataPropertyName = "UseHyperForReverse",
            Width = 100,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None
        });

        // Bind data
        var bindingList = new BindingList<ShortcutDisplayModel>(
            _shortcuts.Select(s => new ShortcutDisplayModel(s)).ToList()
        );
        dataGridView.DataSource = bindingList;

        // Info panel at top
        var infoPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(240, 240, 240)
        };

        var infoLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            Text = "Active Shortcuts\n" +
                   "Meh = Ctrl+Alt+Shift  |  Hyper = Ctrl+Alt+Shift+Win  |  Double-click tray icon or right-click → Show Shortcuts"
        };
        infoPanel.Controls.Add(infoLabel);

        // Button panel at bottom
        var buttonPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            Padding = new Padding(12)
        };

        var addButton = new Button
        {
            Text = "Add Shortcut",
            Size = new Size(120, 35),
            Location = new Point(12, 12),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular)
        };
        addButton.Click += (s, e) => AddShortcut(form, bindingList);

        var editButton = new Button
        {
            Text = "Edit",
            Size = new Size(100, 35),
            Location = new Point(140, 12),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            Enabled = false
        };
        editButton.Click += (s, e) => EditShortcut(dataGridView, bindingList);

        var removeButton = new Button
        {
            Text = "Remove",
            Size = new Size(100, 35),
            Location = new Point(248, 12),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            Enabled = false
        };
        removeButton.Click += (s, e) => RemoveShortcut(dataGridView, bindingList);

        var saveButton = new Button
        {
            Text = "Save & Restart",
            Size = new Size(120, 35),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular)
        };
        saveButton.Click += (s, e) => SaveAndRestart(bindingList);

        var closeButton = new Button
        {
            Text = "Close",
            Size = new Size(100, 35),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular)
        };
        closeButton.Click += (s, e) => form.Close();

        dataGridView.SelectionChanged += (s, e) =>
        {
            var hasSelection = dataGridView.SelectedRows.Count > 0;
            editButton.Enabled = hasSelection;
            removeButton.Enabled = hasSelection;
        };

        dataGridView.CellDoubleClick += (s, e) =>
        {
            if (e.RowIndex >= 0)
            {
                EditShortcut(dataGridView, bindingList);
            }
        };

        // Create a flow layout panel for right-aligned buttons
        var rightButtonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 240,
            Padding = new Padding(0),
            WrapContents = false,
            AutoScroll = false
        };
        rightButtonPanel.Controls.Add(saveButton);
        rightButtonPanel.Controls.Add(closeButton);

        buttonPanel.Controls.Add(addButton);
        buttonPanel.Controls.Add(editButton);
        buttonPanel.Controls.Add(removeButton);
        buttonPanel.Controls.Add(rightButtonPanel);

        form.Controls.Add(dataGridView);
        form.Controls.Add(infoPanel);
        form.Controls.Add(buttonPanel);

        form.ShowDialog();
    }

    private void AddShortcut(Form parentForm, BindingList<ShortcutDisplayModel> bindingList)
    {
        ShowShortcutDialog(null, bindingList, "Add Shortcut", "Add");
    }

    private void EditShortcut(DataGridView dataGridView, BindingList<ShortcutDisplayModel> bindingList)
    {
        if (dataGridView.SelectedRows.Count == 0) return;

        var selectedModel = (ShortcutDisplayModel)dataGridView.SelectedRows[0].DataBoundItem;
        var shortcutToEdit = _shortcuts.FirstOrDefault(s => s.Key == selectedModel.Key);

        if (shortcutToEdit != null)
        {
            ShowShortcutDialog(shortcutToEdit, bindingList, "Edit Shortcut", "Save");
        }
    }

    private void ShowShortcutDialog(ShortcutMapping? existingShortcut, BindingList<ShortcutDisplayModel> bindingList, string title, string okButtonText)
    {
        bool isEdit = existingShortcut != null;
        string? originalKey = existingShortcut?.Key;

        using var dialog = new Form
        {
            Text = title,
            Size = new Size(650, 350),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MinimizeBox = false,
            MaximizeBox = false,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
        };

        int yPos = 15;
        int labelWidth = 160;
        int fieldLeft = 180;
        int fieldWidth = 385;
        int controlHeight = 40;
        int spacing = 30;

        // Key
        var keyLabel = new Label 
        { 
            Text = "Key:", 
            Location = new Point(15, yPos + 3), 
            Width = labelWidth,
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular),
            AutoSize = false
        };
        var keyTextBox = new TextBox 
        { 
            Location = new Point(fieldLeft, yPos), 
            Width = 40, 
            Height = controlHeight,
            MaxLength = 1,
            Text = existingShortcut?.Key ?? "",
            Enabled = true, // Allow editing the key
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular)
        };
        dialog.Controls.Add(keyLabel);
        dialog.Controls.Add(keyTextBox);

        yPos += spacing;

        // Application Path
        var pathLabel = new Label 
        { 
            Text = "Application Path:", 
            Location = new Point(15, yPos + 3), 
            Width = labelWidth,
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular),
            AutoSize = false
        };
        var pathTextBox = new TextBox 
        { 
            Location = new Point(fieldLeft, yPos), 
            Width = fieldWidth - 75,
            Height = controlHeight,
            Text = existingShortcut?.ApplicationPath ?? "",
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
        };
        var browseButton = new Button 
        { 
            Text = "Browse...", 
            Location = new Point(fieldLeft + fieldWidth - 70, yPos - 1), 
            Width = 100,
            Height = 30,
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular)
        };
        browseButton.Click += (s, e) =>
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = "Select Application",
                InitialDirectory = !string.IsNullOrEmpty(pathTextBox.Text) && File.Exists(pathTextBox.Text)
                    ? Path.GetDirectoryName(pathTextBox.Text)
                    : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            if (!string.IsNullOrEmpty(pathTextBox.Text) && File.Exists(pathTextBox.Text))
            {
                openFileDialog.FileName = pathTextBox.Text;
            }

            if (openFileDialog.ShowDialog(dialog) == DialogResult.OK)
            {
                pathTextBox.Text = openFileDialog.FileName;
            }
        };
        dialog.Controls.Add(pathLabel);
        dialog.Controls.Add(pathTextBox);
        dialog.Controls.Add(browseButton);

        yPos += spacing;

        // Working Directory
        var workDirLabel = new Label 
        { 
            Text = "Working Directory:", 
            Location = new Point(15, yPos + 3), 
            Width = labelWidth,
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular),
            AutoSize = false
        };
        var workDirTextBox = new TextBox 
        { 
            Location = new Point(fieldLeft, yPos), 
            Width = fieldWidth - 75,
            Height = controlHeight,
            Text = existingShortcut?.WorkingDirectory ?? "",
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
        };
        var browseDirButton = new Button 
        { 
            Text = "Browse...", 
            Location = new Point(fieldLeft + fieldWidth - 70, yPos - 1), 
            Width = 100,
            Height = 30,
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular)
        };
        browseDirButton.Click += (s, e) =>
        {
            using var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select Working Directory",
                ShowNewFolderButton = true,
                SelectedPath = !string.IsNullOrEmpty(workDirTextBox.Text) && Directory.Exists(workDirTextBox.Text)
                    ? workDirTextBox.Text
                    : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };

            if (folderBrowserDialog.ShowDialog(dialog) == DialogResult.OK)
            {
                workDirTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        };
        dialog.Controls.Add(workDirLabel);
        dialog.Controls.Add(workDirTextBox);
        dialog.Controls.Add(browseDirButton);

        yPos += spacing + 15;

        // Use Meh
        var mehCheckBox = new CheckBox 
        { 
            Text = "Use Meh (Ctrl+Alt+Shift)", 
            Location = new Point(fieldLeft, yPos), 
            Width = 340,
            Height = controlHeight,
            Checked = existingShortcut?.UseMeh ?? true,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
        };
        dialog.Controls.Add(mehCheckBox);

        yPos += 50;

        // Use Hyper for Reverse
        var hyperCheckBox = new CheckBox 
        { 
            Text = "Use Hyper for Reverse (Ctrl+Alt+Shift+Win)", 
            Location = new Point(fieldLeft, yPos), 
            Width = 340,
            Height = controlHeight,
            Checked = existingShortcut?.UseHyperForReverse ?? true,
            Font = new Font("Segoe UI", 8.25F, FontStyle.Regular)
        };
        dialog.Controls.Add(hyperCheckBox);

        yPos += 45;

        // Buttons
        var okButton = new Button
        {
            Text = okButtonText,
            DialogResult = DialogResult.OK,
            Size = new Size(90, 28),
            Location = new Point(fieldLeft + fieldWidth - 186, yPos),
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular)
        };
        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Size = new Size(90, 28),
            Location = new Point(fieldLeft + fieldWidth - 90, yPos),
            Font = new Font("Segoe UI", 7.25F, FontStyle.Regular)
        };

        dialog.Controls.Add(okButton);
        dialog.Controls.Add(cancelButton);
        dialog.AcceptButton = okButton;
        dialog.CancelButton = cancelButton;

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            var key = keyTextBox.Text.Trim().ToUpperInvariant();
            var path = pathTextBox.Text.Trim();
            var workDir = workDirTextBox.Text.Trim();

            // Validate key
            var keyValidation = ConfigValidator.ValidateKey(key);
            if (!keyValidation.IsValid)
            {
                MessageBox.Show(keyValidation.ErrorMessage, "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate path
            var pathValidation = ConfigValidator.ValidatePath(path, "Application path");
            if (!pathValidation.IsValid)
            {
                MessageBox.Show(pathValidation.ErrorMessage, "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate working directory
            if (!string.IsNullOrEmpty(workDir))
            {
                var workDirValidation = ConfigValidator.ValidateWorkingDirectory(workDir);
                if (!workDirValidation.IsValid)
                {
                    MessageBox.Show(workDirValidation.ErrorMessage, "Invalid Working Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Check for duplicate key (but allow same key if editing)
            if (bindingList.Any(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase) && key != originalKey))
            {
                MessageBox.Show($"A shortcut with key '{key}' already exists.", "Duplicate Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isEdit && existingShortcut != null)
            {
                // Update existing shortcut (including key if changed)
                existingShortcut.Key = key;
                existingShortcut.ApplicationPath = path;
                existingShortcut.WorkingDirectory = workDir;
                existingShortcut.UseMeh = mehCheckBox.Checked;
                existingShortcut.UseHyperForReverse = hyperCheckBox.Checked;

                // Refresh the display
                var modelToUpdate = bindingList.FirstOrDefault(m => m.Key == originalKey);
                if (modelToUpdate != null)
                {
                    var index = bindingList.IndexOf(modelToUpdate);
                    bindingList[index] = new ShortcutDisplayModel(existingShortcut);
                }

                MessageBox.Show("Shortcut updated. Click 'Save & Restart' to apply changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Add new shortcut
                var newShortcut = new ShortcutMapping
                {
                    Key = key,
                    ApplicationPath = path,
                    WorkingDirectory = workDir,
                    UseMeh = mehCheckBox.Checked,
                    UseHyperForReverse = hyperCheckBox.Checked
                };

                _shortcuts.Add(newShortcut);
                bindingList.Add(new ShortcutDisplayModel(newShortcut));

                MessageBox.Show("Shortcut added. Click 'Save & Restart' to apply changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void RemoveShortcut(DataGridView dataGridView, BindingList<ShortcutDisplayModel> bindingList)
    {
        if (dataGridView.SelectedRows.Count == 0) return;

        var selectedModel = (ShortcutDisplayModel)dataGridView.SelectedRows[0].DataBoundItem;

        var result = MessageBox.Show(
            $"Remove shortcut for key '{selectedModel.Key}'?",
            "Confirm Remove",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var shortcutToRemove = _shortcuts.FirstOrDefault(s => s.Key == selectedModel.Key);
            if (shortcutToRemove != null)
            {
                _shortcuts.Remove(shortcutToRemove);
                bindingList.Remove(selectedModel);
                MessageBox.Show("Shortcut removed. Click 'Save & Restart' to apply changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void SaveAndRestart(BindingList<ShortcutDisplayModel> bindingList)
    {
        try
        {
            ConfigStore.Save(new AppShortcutConfig { Shortcuts = _shortcuts });
            Log.Info($"Saved {_shortcuts.Count} shortcut(s) to {AppPaths.ConfigFile}");

            var result = MessageBox.Show(
                "Configuration saved. The application needs to restart to apply changes.\n\nRestart now?",
                "Restart Required",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error saving configuration", ex);
            MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Helper class for DataGridView binding
    private class ShortcutDisplayModel
    {
        private readonly ShortcutMapping _mapping;

        public ShortcutDisplayModel(ShortcutMapping mapping)
        {
            _mapping = mapping;
        }

        public string Key => _mapping.Key;
        public string ApplicationName => Path.GetFileNameWithoutExtension(_mapping.ApplicationPath);
        public string ApplicationPath => _mapping.ApplicationPath;
        public bool UseMeh => _mapping.UseMeh;
        public bool UseHyperForReverse => _mapping.UseHyperForReverse;
    }

    private void OnExit(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        Application.Exit();
    }

    private void OnSessionEnded(object? sender, SessionEndedEventArgs e)
    {
        Log.Info($"Session ending ({e.Reason}); exiting");
        _trayIcon.Visible = false;
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SystemEvents.SessionEnded -= OnSessionEnded;
            _trayIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}
