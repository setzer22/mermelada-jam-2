using System;
using Godot;

public class Surface : Node
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

    public Vector2 PlaceItemPos =>
        (
            Prop.GetNode<Position2D>("PlaceItemPos")
            ?? throw new Exception("Surface scenes should have a PlaceItemPos child node")
        ).GlobalPosition;

    public ItemProp itemOnTop = null;

    public override void _Ready()
    {
        // Try to find an item on top that was placed during level creation.
        foreach (var ch in GetChildren())
        {
            if (ch is ItemProp p)
            {
                itemOnTop = p;
                break;
            }
        }
    }

    public bool HasItem()
    {
        return itemOnTop != null;
    }
}
