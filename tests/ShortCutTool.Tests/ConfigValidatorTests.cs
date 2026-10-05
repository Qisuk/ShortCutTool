using ShortCutTool;
using Xunit;

namespace ShortCutTool.Tests;

public class ConfigValidatorTests
{
    private static ShortcutMapping Valid(string key = "N") => new()
    {
        Key = key,
        UseMeh = true,
        ApplicationPath = @"C:\Windows\System32\notepad.exe"
    };

    [Fact]
    public void NullConfig_IsInvalid()
    {
        Assert.False(ConfigValidator.ValidateConfig(null).IsValid);
    }

    [Fact]
    public void EmptyShortcutList_IsValid()
    {
        // First run starts with an empty list and opens the Shortcut Manager.
        Assert.True(ConfigValidator.ValidateConfig(new AppShortcutConfig()).IsValid);
    }

    [Fact]
    public void SingleValidShortcut_IsValid()
    {
        var config = new AppShortcutConfig { Shortcuts = { Valid() } };
        Assert.True(ConfigValidator.ValidateConfig(config).IsValid);
    }

    [Fact]
    public void DuplicateKeys_IgnoringCase_AreInvalid()
    {
        var config = new AppShortcutConfig { Shortcuts = { Valid("n"), Valid("N") } };
        var result = ConfigValidator.ValidateConfig(config);

        Assert.False(result.IsValid);
        Assert.Contains("Duplicate", result.ErrorMessage);
    }

    [Fact]
    public void MoreThan100Shortcuts_AreInvalid()
    {
        var config = new AppShortcutConfig();
        for (var i = 0; i < 101; i++)
        {
            config.Shortcuts.Add(Valid(((char)('A' + i % 26)).ToString()));
        }

        Assert.False(ConfigValidator.ValidateConfig(config).IsValid);
    }

    [Fact]
    public void InvalidShortcut_ErrorNamesItsPosition()
    {
        var config = new AppShortcutConfig { Shortcuts = { Valid("A"), Valid("!") } };
        var result = ConfigValidator.ValidateConfig(config);

        Assert.False(result.IsValid);
        Assert.StartsWith("Shortcut 2:", result.ErrorMessage);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("z")]
    [InlineData("7")]
    public void ValidateKey_AcceptsSingleAlphanumeric(string key)
    {
        Assert.True(ConfigValidator.ValidateKey(key).IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("AB")]
    [InlineData("!")]
    [InlineData("é")]
    public void ValidateKey_RejectsEverythingElse(string? key)
    {
        Assert.False(ConfigValidator.ValidateKey(key).IsValid);
    }

    [Theory]
    [InlineData(@"C:\Windows\System32\notepad.exe")]
    [InlineData(@"%WINDIR%\System32\notepad.exe")]
    [InlineData(@"C:\Users\%USERNAME%\AppData\Local\Programs\Microsoft VS Code\Code.exe")]
    public void ValidatePath_AcceptsAbsoluteAndEnvironmentPaths(string path)
    {
        Assert.True(ConfigValidator.ValidatePath(path).IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(@"C:\Apps\..\Windows\notepad.exe")]
    [InlineData(@"\\server\share\app.exe")]
    [InlineData("/usr/bin/app")]
    [InlineData(@"C:\Apps\a|b.exe")]
    [InlineData("CON")]
    [InlineData(@"relative\app.exe")]
    public void ValidatePath_RejectsUnsafeOrRelativePaths(string? path)
    {
        Assert.False(ConfigValidator.ValidatePath(path).IsValid);
    }

    [Fact]
    public void ValidatePath_RejectsOverlongPaths()
    {
        var path = @"C:\" + new string('a', 500) + ".exe";
        Assert.False(ConfigValidator.ValidatePath(path).IsValid);
    }

    [Fact]
    public void EmptyWorkingDirectory_IsValid()
    {
        Assert.True(ConfigValidator.ValidateWorkingDirectory("").IsValid);
        Assert.True(ConfigValidator.ValidateWorkingDirectory(null).IsValid);
    }

    [Fact]
    public void SanitizePath_ExpandsEnvironmentVariables()
    {
        var windir = Environment.GetEnvironmentVariable("WINDIR")!;
        var result = ConfigValidator.SanitizePath(@"%WINDIR%/System32/notepad.exe");

        Assert.Equal(Path.Combine(windir, "System32", "notepad.exe"), result, ignoreCase: true);
    }
}
