namespace ShortCutTool;

public class AppShortcutConfig
{
    public List<ShortcutMapping> Shortcuts { get; set; } = new();
}

public class ShortcutMapping
{
    public string Key { get; set; } = string.Empty;
    public bool UseMeh { get; set; } = true;
    public bool UseHyperForReverse { get; set; } = false;
    public string ApplicationPath { get; set; } = string.Empty;
    public string? WorkingDirectory { get; set; }
}