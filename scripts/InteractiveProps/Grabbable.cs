using System;
using Godot;

public interface IGrabber
{
    Vector2 Position { get; }
    Grabbable GrabbedObject { set; }
    void AddChild(Node node, bool legibleUniqueName = false);
}

public class Grabbable : Node
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

    Tween GrabTween;

    public override void _Ready()
    {
        var tween = new Tween();
        AddChild(tween);
        GrabTween = tween;
    }

    public void Grab(IGrabber grabber, Vector2 grabberOffs = default)
    {
        // Reparent to a different node while keeping the same global position
        var globalPos = Prop.GlobalPosition;
        var parentProp = Prop.GetParent();
        parentProp.RemoveChild(Prop);
        grabber.AddChild(Prop);
        Prop.GlobalPosition = globalPos;

        // If our parent is a surface, notify it that we just got yoinked
        if (parentProp is ItemProp ip && ip.SurfaceComponent() is Surface surface)
        {
            surface.itemOnTop = null;
        }

        // We start a tween to move the object towards the grabber
        GrabTween.InterpolateProperty(
            Prop,
            "position",
            Prop.Position,
            grabberOffs,
            0.25f,
            // Yoink!
            Tween.TransitionType.Cubic,
            Tween.EaseType.InOut
        );
        GrabTween.Start();

        // When the tween ends, we notify the grabber
        GrabTween.Connect(
            "tween_all_completed",
            this,
            nameof(OnGrabTweenCompleted),
            new Godot.Collections.Array { grabber }
        );

        // While grabbed, the parent collider is disabled
        DisableCollisions();
        SoundManager.Singleton.PlayOneShot_typed(SFXSoundType.GrabBig);
    }

    public void Drop(Surface surface)
    {
        // Reparent to the surface
        var globalPos = Prop.GlobalPosition;
        var parentProp = Prop.GetParent();
        parentProp.RemoveChild(Prop);
        surface.Prop.AddChild(Prop);
        Prop.GlobalPosition = globalPos;

        // We start a tween to move the object towards the surface
        GrabTween.InterpolateProperty(
            Prop,
            "global_position",
            Prop.GlobalPosition,
            surface.PlaceItemPos,
            0.25f,
            Tween.TransitionType.Cubic,
            Tween.EaseType.InOut
        );
        GrabTween.Start();

        // When the tween ends, we notify the surface
        GrabTween.Connect(
            "tween_all_completed",
            this,
            nameof(OnDropTweenCompleted),
            new Godot.Collections.Array { surface }
        );
    }

    public void OnGrabTweenCompleted(object grabberObj)
    {
        var grabber = grabberObj as IGrabber ?? throw new Exception("Expected IGrabber");
        grabber.GrabbedObject = this;
        GrabTween.Disconnect("tween_all_completed", this, nameof(OnGrabTweenCompleted));
    }

    public void OnDropTweenCompleted(object surfaceObj)
    {
        var surface = surfaceObj as Surface ?? throw new Exception("Expected Surface");
        surface.itemOnTop = Prop;
        EnableCollisions();
        GrabTween.Disconnect("tween_all_completed", this, nameof(OnDropTweenCompleted));
    }

    public void DisableCollisions()
    {
        foreach (var ch in Prop.GetNode("Collisions").GetChildren())
        {
            if (ch is CollisionShape2D shape)
            {
                shape.Disabled = true;
            }
            else if (ch is CollisionPolygon2D poly)
            {
                poly.Disabled = true;
            }
        }
    }

    public void EnableCollisions()
    {
        foreach (var ch in Prop.GetNode("Collisions").GetChildren())
        {
            if (ch is CollisionShape2D shape)
            {
                shape.Disabled = false;
            }
            else if (ch is CollisionPolygon2D poly)
            {
                poly.Disabled = false;
            }
        }
    }
}
