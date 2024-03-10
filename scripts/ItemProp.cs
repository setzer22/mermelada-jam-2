using System;
using Godot;

[Tool]
public class ItemProp : Node2D
{
    [Export]
    public bool highlighted = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    public override void _Process(float delta)
    {
        // Code that runs both in the editor and game goes here
        var material = GetNode<Sprite>("Base").Material as ShaderMaterial;
        material.SetShaderParam("width", highlighted ? 1.2 : 0);

        if (!Engine.EditorHint)
        {
            // Game code goes here!
        }
    }

    private T GetInteractionComponent<T>(string nodeName)
    {
        if (HasNode(nodeName))
        {
            if (GetNode(nodeName) is T t)
            {
                return t;
            }
            else
            {
                throw new Exception($"{nodeName} node is not {typeof(T).Name}?");
            }
        }
        return default;
    }

    public Grabbable GrabbableComponent()
    {
        return GetInteractionComponent<Grabbable>("Grabbable");
    }

    public Surface SurfaceComponent()
    {
        return GetInteractionComponent<Surface>("Surface");
    }
}
