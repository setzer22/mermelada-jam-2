using System;
using System.Collections.Generic;
using Godot;

public class HouseRating : TextureRect
{
    
    [Signal]
    public delegate void Finished();
    
    private static List<string> badRatingStrings = new List<string>()
    {
        "Las energías son\nun poco turbias\n",
        "Las cosas no se quedan\ndonde las dejas\n",
        "La electricidad falla\nmucho sin motivo\n",
        "Los muebles hacen ruidos\ny se abren solos\n",
        "Decían que habia\n fantasmas pero era\nmentira",
    };

    [Export]
    public int NumBadRatings = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PlayAnimation();
    }

    public async void PlayAnimation()
    {
        var vbox = FindNode("VBoxContainer") ?? throw new Exception("No vbox??");

        for (int i = 0; i < NumBadRatings - 1; ++i)
        {
            var label = vbox.GetChild<Label>(i);
            label.Text = badRatingStrings[i];
            label.Set("custom_colors/font_color", new Color("ffa71212"));
        }

        if (NumBadRatings > 0)
        {
            // Fade out the current rating in green, then fade in the bad one in red
            var tw = new Tween();
            var currLabel = vbox.GetChild<Label>(NumBadRatings - 1);

            AddChild(tw);
            tw.InterpolateProperty(
                currLabel,
                "custom_colors/font_color",
                new Color("ff467a0d"),
                new Color("00467a0d"),
                1f,
                Tween.TransitionType.Quad,
                Tween.EaseType.InOut
            );
            tw.Start();

            await ToSignal(tw, "tween_completed");
            
            currLabel.Text = badRatingStrings[NumBadRatings - 1];
            
            tw.InterpolateProperty(
                currLabel,
                "custom_colors/font_color",
                new Color("00a71212"),
                new Color("ffa71212"),
                1f,
                Tween.TransitionType.Quad,
                Tween.EaseType.InOut
            );
            tw.Start();
            
            await ToSignal(tw, "tween_completed");
            
            EmitSignal(nameof(Finished));
        }
    }
}
