using System;
using System.Runtime.Versioning;
using Godot;

public class Player : KinematicBody2D
{
    private float elapsed = 0f;
    private Vector2 velocity = new Vector2();
    private float timeToNextBlink = 0f;
    
    // Store the initial sprite translation so that we can add an offset to it
    private Vector2 initialSpriteTranslation;

    [Export]
    private float FloatAnimAmplitude = 20f;

    [Export]
    private float FloatAnimFrequency = 2f;

    [Export]
    private float MoveSpeed = 150f;
    
    [Export]
    private float BlinkMin = 2.5f;
    
    [Export]
    private float BlinkMax = 8f;

    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        initialSpriteTranslation = GetNode<Node2D>("Sprite").Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        elapsed += delta;

        // Float animation
        var sprite = GetNode<Node2D>("Sprite");
        sprite.Position =
            initialSpriteTranslation
            + Vector2.Up * Mathf.Sin(elapsed * FloatAnimFrequency) * FloatAnimAmplitude;

        // Flip sprite towards movement direction
        if (velocity.x != 0)
        {
            foreach (var ch in sprite.GetChildren())
            {
                if (ch is AnimatedSprite s)
                {
                    s.FlipH = velocity.x > 0;
                }
            }
        }

        if (timeToNextBlink > 0)
        {
            timeToNextBlink -= delta;
        }
        else
        {
            Blink();
            timeToNextBlink = (float)GD.RandRange(BlinkMin, BlinkMax);
        }
    }

    // Don't
    public void Blink()
    {
        var eyes = GetNode<AnimatedSprite>("Sprite/Eyes");
        eyes.Frame = 0;
        eyes.Playing = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        var movement = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        velocity = movement.Normalized() * MoveSpeed;
        velocity = MoveAndSlide(velocity);
    }
}
