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
        var titleSteps = 10;
        var titleDurationSecs = 1f;
        for (int i = 0; i < 10; ++i)
        {
            title.PercentVisible += 1f / titleSteps;
            await ToSignal(GetTree().CreateTimer(titleDurationSecs / titleSteps), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        // Animate subtitle
        var subtitleSteps = 10;
        var subtitleDurationSecs = 1f;
        for (int i = 0; i < 10; ++i)
        {
            subtitle.PercentVisible += 1f / subtitleSteps;
            await ToSignal(GetTree().CreateTimer(subtitleDurationSecs / subtitleSteps), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        if (IsOutro)
        {
            // Animate evicted label
            var evictedSteps = 10;
            var evictedDurationSecs = 2f;
            for (int i = 0; i < 10; ++i)
            {
                evictedLabel.PercentVisible += 1f / evictedSteps;
                await ToSignal(
                    GetTree().CreateTimer(evictedDurationSecs / evictedSteps),
                    "timeout"
                );
            }
        }

        await ToSignal(GetTree().CreateTimer(2f, false), "timeout");

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
