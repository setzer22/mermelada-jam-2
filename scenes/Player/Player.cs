using System;
using Godot;

public class Player : KinematicBody2D
{
    private float elapsed = 0f;
    private Vector2 velocity = new Vector2();

    // Store the initial sprite translation so that we can add an offset to it
    private Vector2 initialSpriteTranslation;

    [Export]
    private float FloatAnimAmplitude = 20f;

    [Export]
    private float FloatAnimFrequency = 2f;

    [Export]
    private float MoveSpeed = 150f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        initialSpriteTranslation = GetNode<Sprite>("Sprite").Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        elapsed += delta;

        // Float animation
        var sprite = GetNode<Sprite>("Sprite");
        sprite.Position =
            initialSpriteTranslation
            + Vector2.Up * Mathf.Sin(elapsed * FloatAnimFrequency) * FloatAnimAmplitude;
        
        // Flip sprite towards movement direction
        if (velocity.x != 0)
        {
            sprite.FlipH = velocity.x > 0;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        var movement = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        velocity = movement.Normalized() * MoveSpeed;
        velocity = MoveAndSlide(velocity);
    }
}
