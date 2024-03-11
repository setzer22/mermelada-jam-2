using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Godot;

public class Player : KinematicBody2D, IGrabber
{
    private float elapsed = 0f;
    private Vector2 velocity = new Vector2();
    private float timeToNextBlink = 0f;
    private List<ItemProp> interactablesInRange = new List<ItemProp>();
    private int selectedInteractableIdx = 0;
    private float sayTextElapsed = 0f;
    private bool sayingText = false;

    private Grabbable grabbedObject;
    public Grabbable GrabbedObject
    {
        set { grabbedObject = value; }
    }

    private Label _sayText;
    private Label SayText => _sayText ??= GetNode<Label>("SayText");

    private Node2D _sprite;
    private Node2D Sprite => _sprite ??= GetNode<Node2D>("Sprite");

    private Area2D _influenceRadius;
    private Area2D InfluenceRadius => _influenceRadius ??= GetNode<Area2D>("InfluenceRadius");

    private Position2D _heldObjectPos;
    private Position2D HeldObjectPos => _heldObjectPos ??= GetNode<Position2D>("HeldObjectPos");

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

    [Export]
    private float Opacity = 0.9f;

    [Export]
    private float OpacityAnimAmplitude = 0.1f;

    [Export]
    private float OpacityAnimFrequency = 1f;

    [Export]
    private float MessageTime = 2f;

    [Export]
    private float MessageClearTime = 5f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        initialSpriteTranslation = GetNode<Node2D>("Sprite").Position;

        InfluenceRadius.Connect("body_entered", this, nameof(_OnBodyEnteredInfluenceRadius));
        InfluenceRadius.Connect("body_exited", this, nameof(_OnBodyExitedInfluenceRadius));

        SayText.Text = "";
    }

    public static ItemProp BodyToItemProp(Node body)
    {
        // The prop is typically the parent of the collider, we check both just
        // in case...
        if (body is ItemProp item)
        {
            return item;
        }
        else if (body.GetParent() is ItemProp item_)
        {
            return item_;
        }
        return null;
    }

    public void _OnBodyEnteredInfluenceRadius(Node body)
    {
        selectedInteractableIdx = 0;
        var itemProp = BodyToItemProp(body);
        if (itemProp != null)
        {
            interactablesInRange.Add(itemProp);
        }
    }

    public void _OnBodyExitedInfluenceRadius(Node body)
    {
        var itemProp = BodyToItemProp(body);
        if (itemProp != null)
        {
            itemProp.highlighted = false;
            interactablesInRange.Remove(itemProp);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        elapsed += delta;

        // Float animation
        var floatYOffset =
            Vector2.Up * Mathf.Sin(elapsed * FloatAnimFrequency) * FloatAnimAmplitude;
        Sprite.Position = initialSpriteTranslation + floatYOffset;

        var newOpacity = Opacity + Mathf.Sin(elapsed * OpacityAnimFrequency) * OpacityAnimAmplitude;
        Sprite.Modulate = new Color(1, 1, 1, newOpacity);

        // Flip sprite towards movement direction
        if (Mathf.Abs(velocity.x) > 20f)
        {
            foreach (var ch in Sprite.GetChildren())
            {
                if (ch is AnimatedSprite s)
                {
                    s.FlipH = velocity.x > 0;
                }
            }

            var hpos = HeldObjectPos.Position;
            hpos.x = velocity.x > 0 ? Mathf.Abs(hpos.x) : -Mathf.Abs(hpos.x);
            HeldObjectPos.Position = hpos;
        }

        // Random blink
        if (timeToNextBlink > 0)
        {
            timeToNextBlink -= delta;
        }
        else
        {
            Blink();
            timeToNextBlink = (float)GD.RandRange(BlinkMin, BlinkMax);
        }

        // Interactables
        if (interactablesInRange.Count > 0)
        {
            // Allow cycling interactables
            if (Input.IsActionJustPressed("NextInteractable"))
            {
                selectedInteractableIdx += 1;
            }
            selectedInteractableIdx %= interactablesInRange.Count;

            // Highlight the closest interactable
            interactablesInRange.ForEach(i => i.highlighted = false);
            var selected = interactablesInRange
                .OrderBy(i => (i.GlobalPosition - GlobalPosition).LengthSquared())
                .Skip(selectedInteractableIdx)
                .First();
            selected.highlighted = true;

            HandleInteraction(selected);
        }

        // Grabbed item
        if (this.grabbedObject != null)
        {
            this.grabbedObject.Prop.Position = HeldObjectPos.Position + floatYOffset * 0.8f;
        }

        // Speaking via text
        if (this.sayingText)
        {
            this.sayTextElapsed += delta;
            SayText.PercentVisible = this.sayTextElapsed / MessageTime;

            if (this.sayTextElapsed > MessageClearTime)
            {
                this.sayingText = false;
                SayText.Text = "";
                SayText.PercentVisible = 0f;
            }
        }

        if (Input.IsActionJustPressed("Debug"))
        {
            this.Say("Esto aquí no va...");
        }
    }

    public void HandleInteraction(ItemProp selected)
    {
        // The interaction system uses composition. This means a single item
        // prop can have multiple interaction modes.
        var handled = false;
        var errorSentence = "";

        if (Input.IsActionJustPressed("Interact"))
        {
            if (grabbedObject == null && !handled && selected.GrabbableComponent() is Grabbable g)
            {
                g.Grab(this, HeldObjectPos.Position);
                handled = true;
            }

            if (!handled && selected.SurfaceComponent() is Surface s)
            {
                if (this.grabbedObject != null)
                {
                    if (!s.HasItem())
                    {
                        this.grabbedObject.Drop(s);

                        GameManager.Singleton.PlaceItemAction(this.grabbedObject, s);

                        this.grabbedObject = null;
                        handled = true;
                    }
                    else
                    {
                        errorSentence = "No puedo poner más objetos aquí...";
                    }
                }
                else
                {
                    errorSentence = "No tengo nada que poner aquí...";
                }
            }

            if (!handled && selected.SwitchableComponent() is Switchable sw)
            {
                sw.Switch(this.grabbedObject?.Key);

                GameManager.Singleton.SwitchedItemAction(sw, this.grabbedObject);
                
                // ULTRA-HACK: This shouldn't go here, too bad it's 1h30m before submission deadline
                if (sw.Key == "tv" && this.grabbedObject?.Key == "blanket") {
                    this.grabbedObject.Prop.QueueFree();
                    this.grabbedObject = null;
                }

                handled = true;
            }

            // If action could not be handled, we give the most reasonable error sentence
            if (!handled && errorSentence != "")
            {
                this.Say(errorSentence);
            }
            else if (!handled)
            {
                this.Say("Esto no va a hacer nada...");
            }
        }
    }

    public void Say(string text)
    {
        SayText.Text = text;
        SayText.PercentVisible = 0f;
        this.sayTextElapsed = 0f;
        this.sayingText = true;
    }

    // Don't
    public void Blink()
    {
        var eyes = Sprite.GetNode<AnimatedSprite>("Eyes");
        eyes.Frame = 0;
        eyes.Playing = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        var movement = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        velocity = movement.Normalized() * MoveSpeed;
        velocity = MoveAndSlide(velocity);
    }
}
