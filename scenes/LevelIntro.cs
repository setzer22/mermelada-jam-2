using System;
using Godot;

public class LevelIntro : Control
{
    [Signal]
    delegate void Finished();

    [Export]
    public string OutroText = "<TODO OUTRO TEXT>";

    [Export]
    public bool IsOutro = false;
    
    [Export]
    public float TitleSteps = 10;

    [Export]
    public float TitleDurationSecs = 1f;

    [Export]
    public float SubtitleSteps = 10;

    [Export]
    public float SubtitleDurationSecs = 1f;
    
    [Export]
    public float EvictedSteps = 10;
    
    [Export]
    public float EvictedDurationSecs = 2f;
    
    [Export]
    public float EndWait = 2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AnimateText();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("CheatSkipIntro")) {
            EmitSignal(nameof(Finished));
        }
    }

    public async void AnimateText()
    {
        var title = GetNode<Label>("Title");
        var subtitle = GetNode<Label>("Subtitle");
        var evictedLabel = GetNode<Label>("EvictedLabel");
        var tenant = GetNode<Node2D>("Tenant");

        title.PercentVisible = 0f;
        subtitle.PercentVisible = 0f;
        evictedLabel.PercentVisible = 0f;

        if (IsOutro)
        {
            subtitle.Text = OutroText;
        }

        // Fade in tenant
        tenant.Modulate = new Color(1, 1, 1, 0);
        var tw = new Tween();
        AddChild(tw);
        tw.InterpolateProperty(
            tenant,
            "modulate",
            new Color(1, 1, 1, 0),
            new Color(1, 1, 1, 1),
            0.5f,
            Tween.TransitionType.Quad,
            Tween.EaseType.InOut
        );

        tw.Start();
        await ToSignal(tw, "tween_completed");

        // Animate title
        for (int i = 0; i < TitleSteps; ++i)
        {
            title.PercentVisible += 1f / TitleSteps;
            await ToSignal(GetTree().CreateTimer(TitleDurationSecs / TitleSteps), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        // Animate subtitle
        for (int i = 0; i < SubtitleSteps; ++i)
        {
            subtitle.PercentVisible += 1f / SubtitleSteps;
            await ToSignal(GetTree().CreateTimer(SubtitleDurationSecs / SubtitleSteps), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        if (IsOutro)
        {
            // Animate evicted label
            for (int i = 0; i < EvictedSteps; ++i)
            {
                evictedLabel.PercentVisible += 1f / EvictedSteps;
                await ToSignal(
                    GetTree().CreateTimer(EvictedDurationSecs / EvictedSteps),
                    "timeout"
                );
            }
        }

        await ToSignal(GetTree().CreateTimer(EndWait, false), "timeout");

        var tw2 = new Tween();
        AddChild(tw2);
        tw2.InterpolateProperty(
            this,
            "modulate",
            new Color(1, 1, 1, 1),
            new Color(1, 1, 1, 0),
            0.5f
        );
        tw2.Start();

        await ToSignal(tw2, "tween_completed");

        EmitSignal(nameof(Finished));
    }
}
