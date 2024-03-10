using System;
using Godot;

public class Switchable : Node
{
    /// <summary>
    /// The key is used for the purposes of identifying this object to do
    /// certain special effects. If this does not have any specific behavior, it
    /// can be left blank.
    /// </summary>
    [Export]
    public string Key = "";

    public ItemProp Prop =>
        GetParent() as ItemProp ?? throw new Exception("Component should be child of ItemProp");

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
