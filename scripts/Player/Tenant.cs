using System;
using Godot;

public class Tenant : Node2D
{
    Control _fearBox;
    Control FearBox => _fearBox ??= GetNode<Control>("FearBox");

    Area2D _areaOfInfluence;
    Area2D AreaOfInfluence => _areaOfInfluence ??= GetNode<Area2D>("AreaOfInfluence");

    Tween tween;

    public override void _Ready()
    {
        AreaOfInfluence.Connect("body_entered", this, nameof(_OnBodyEntered));
        AreaOfInfluence.Connect("body_exited", this, nameof(_OnBodyExited));
        tween = new Tween();
        AddChild(tween);
        FearBox.Visible = false;
    }

    public void _OnBodyEntered(Node body)
    {
        if (body.IsInGroup("Player"))
        {
            // TODO: someday fearbox...
            //this.ShowFearBox();
        }
    }

    public void _OnBodyExited(Node body)
    {
        if (body.IsInGroup("Player"))
        {
            // TODO: someday fearbox...
            //this.ShowFearBox();
            this.HideFearBox();
        }
    }

    public void ShowFearBox()
    {
        FearBox.Visible = true;
        FearBox.Modulate = new Color(1, 1, 1, 0);
        tween.InterpolateProperty(
            FearBox,
            "modulate",
            new Color(1, 1, 1, 0),
            new Color(1, 1, 1, 1),
            0.5f,
            Tween.TransitionType.Quint,
            Tween.EaseType.InOut
        );
        tween.Start();
    }

    public async void HideFearBox()
    {
        tween.InterpolateProperty(
            FearBox,
            "modulate",
            new Color(1, 1, 1, 1),
            new Color(1, 1, 1, 0),
            0.5f,
            Tween.TransitionType.Quint,
            Tween.EaseType.InOut
        );
        tween.Start();
        
        await ToSignal(tween, "tween_completed");
        FearBox.Visible = false;
    }
}
