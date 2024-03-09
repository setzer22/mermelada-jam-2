using System;
using Godot;

public class ItemProp : Node2D
{
    [Export]
    private bool highlighted = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    public override void _Process(float delta)
    {
        var material = GetNode<Sprite>("Base").Material as ShaderMaterial;
        material.SetShaderParam("width", highlighted ? 1.2 : 0);
    }
}