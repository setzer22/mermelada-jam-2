using Godot;
using System;

public class EndGame : Control
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        var tw = new Tween();
        AddChild(tw);
        this.Modulate = new Color(1, 1, 1, 0);
        tw.InterpolateProperty(
            this,
            "modulate",
            new Color(1, 1, 1, 0),
            new Color(1, 1, 1, 1),
            5f,
            Tween.TransitionType.Quad,
            Tween.EaseType.InOut
        );
        tw.Start();
    }
}
