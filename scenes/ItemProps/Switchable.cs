using System;
using Godot;

public class Switchable : Node
{
    /// <summary>
    /// This signal is fired when the switchable changes state. It can be used
    /// to toggle some cosmetic properties in a script that listens to this
    /// signal.
    /// </summary>
    [Signal]
    delegate void StateChanged(bool isOn);

    [Export]
    private bool IsOn = true;

    public void Switch()
    {
        IsOn = !IsOn;
        EmitSignal(nameof(StateChanged), IsOn);
    }
}
