namespace ShortCutTool;

/// <summary>
/// Manages the state of modifier keys (Ctrl, Alt, Shift, Win) for keyboard input tracking.
/// Provides simplified methods to update and query modifier key states.
/// </summary>
internal class ModifierKeyState
{
    private bool _ctrlPressed;
    private bool _altPressed;
    private bool _shiftPressed;
    private bool _winPressed;

    /// <summary>
    /// Gets a value indicating whether the Ctrl modifier key is currently pressed.
    /// </summary>
    public bool CtrlPressed => _ctrlPressed;

    /// <summary>
    /// Gets a value indicating whether the Alt modifier key is currently pressed.
    /// </summary>
    public bool AltPressed => _altPressed;

    /// <summary>
    /// Gets a value indicating whether the Shift modifier key is currently pressed.
    /// </summary>
    public bool ShiftPressed => _shiftPressed;

    /// <summary>
    /// Gets a value indicating whether the Win modifier key is currently pressed.
    /// </summary>
    public bool WinPressed => _winPressed;

    /// <summary>
    /// Gets a value indicating whether the Meh combination (Ctrl+Alt+Shift, no Win) is pressed.
    /// </summary>
    public bool IsMehPressed => _ctrlPressed && _altPressed && _shiftPressed && !_winPressed;

    /// <summary>
    /// Gets a value indicating whether the Hyper combination (Ctrl+Alt+Shift+Win) is pressed.
    /// </summary>
    public bool IsHyperPressed => _ctrlPressed && _altPressed && _shiftPressed && _winPressed;

    /// <summary>
    /// Gets a value indicating whether any modifier key combination is currently active.
    /// </summary>
    public bool AnyModifierPressed => _ctrlPressed || _altPressed || _shiftPressed || _winPressed;

    /// <summary>
    /// Updates the modifier key state based on the provided key name.
    /// Handles both left, right, and generic modifier key names.
    /// </summary>
    /// <param name="keyName">The name of the key that was pressed (should be uppercase)</param>
    public void HandleKeyDown(string keyName)
    {
        switch (keyName)
        {
            case "LCONTROLKEY" or "RCONTROLKEY" or "CONTROLKEY":
                _ctrlPressed = true;
                break;
            case "LMENU" or "RMENU" or "MENU":
                _altPressed = true;
                break;
            case "LSHIFTKEY" or "RSHIFTKEY" or "SHIFTKEY":
                _shiftPressed = true;
                break;
            case "LWIN" or "RWIN":
                _winPressed = true;
                break;
        }
    }

    /// <summary>
    /// Releases the modifier key state based on the provided key name.
    /// Handles both left, right, and generic modifier key names.
    /// </summary>
    /// <param name="keyName">The name of the key that was released (should be uppercase)</param>
    public void HandleKeyUp(string keyName)
    {
        switch (keyName)
        {
            case "LCONTROLKEY" or "RCONTROLKEY" or "CONTROLKEY":
                _ctrlPressed = false;
                break;
            case "LMENU" or "RMENU" or "MENU":
                _altPressed = false;
                break;
            case "LSHIFTKEY" or "RSHIFTKEY" or "SHIFTKEY":
                _shiftPressed = false;
                break;
            case "LWIN" or "RWIN":
                _winPressed = false;
                break;
        }
    }

    /// <summary>
    /// Resets all modifier key states to not pressed.
    /// Useful for cleanup or error recovery.
    /// </summary>
    public void Reset()
    {
        _ctrlPressed = false;
        _altPressed = false;
        _shiftPressed = false;
        _winPressed = false;
    }
}
