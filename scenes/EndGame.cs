using System;
using Godot;

public class EndGame : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayEndGameAnim();
    }

    public async void PlayEndGameAnim()
    {
        var endPic = GetNode<TextureRect>("FotoEnd");
        var credits = GetNode<MarginContainer>("Credits");
        var endStory = GetNode<Label>("EndStory");

        endPic.Visible = false;
        credits.Visible = false;
        endStory.Visible = true;

        // Animate end story
        var endStorySteps = 60;
        var durationSecs = 4f;
        endStory.PercentVisible = 0f;
        for (int i = 0; i < endStorySteps; ++i)
        {
            endStory.PercentVisible += 1f / endStorySteps;
            await ToSignal(GetTree().CreateTimer(durationSecs / endStorySteps), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");

        var tw = new Tween();
        AddChild(tw);

        // Fade out
        tw.InterpolateProperty(
            this,
            "modulate",
            new Color(1, 1, 1, 1),
            new Color(1, 1, 1, 0),
            5f,
            Tween.TransitionType.Quad,
            Tween.EaseType.InOut
        );

        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

        endPic.Visible = true;
        credits.Visible = true;
        endStory.Visible = false;

        this.Modulate = new Color(1, 1, 1, 0);

        // Fade back in
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
