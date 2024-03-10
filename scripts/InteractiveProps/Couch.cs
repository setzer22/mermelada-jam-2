using Godot;
using System;
using System.Linq; 

public class Couch : Area2D, IInteractiveProp
{
    public event Action<InteractionLevel> InteractionEmitted = delegate { };

    public void Interact()
    {
        InteractionEmitted(InteractionLevel.cosmetic);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SceneInventoryData.Instance.AddProp(this);
        InteractionEmitted += delegate { GD.Print("interaction mock with couch"); };
    }

    public override void _Process(float delta)
    {
        if (GetOverlappingAreas()
            .Cast<KinematicBody2D>()
            .Any(a => a.TryGetPlayer(out Player player)))
        {
            GD.Print("player detected");
            InteractionEmitted(InteractionLevel.cosmetic);
        }
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
