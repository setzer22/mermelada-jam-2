using System;
using Godot;

public class GameManager : Node
{
    public static GameManager Singleton;

    [Signal]
    delegate void InkSpent(float amount);

    [Signal]
    delegate void NewJournalAction(string sentence);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("INITIALIZING GAME MANAGER"); 

        // Handle the singleton on the C# side. GDScript side can access via an
        // Autoload property.
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new Exception("Can't have more than one GameManager");
        }
    }

    /// <summary>
    /// Called when the player places an item on top of a surface
    /// </summary>
    public void PlaceItemAction(Grabbable item, Surface surface)
    {
        var sentence = "";
        if (item.Key == "blanket" && surface.Key == "tv")
        {
            sentence = "Cubrí la tele con una manta";
        }

        // More special actions here...

        if (sentence == "")
        {
            sentence =
                $"Coloqué {item.Prop.HumanReadableName} sobre {surface.Prop.HumanReadableName}";
        }

        GD.Print(sentence);
        EmitSignal(nameof(NewJournalAction), sentence);
    }

    /// <summary>
    /// Called when the player interacts with a switchable item. Grabbed may be
    /// null, if not, it's the item the player was currently grabbing.
    /// </summary>
    public void SwitchedItemAction(Switchable switched, Grabbable grabbed)
    {
        var sentence = "";

        // Drop beer on top of the plant
        if (switched.Key == "plant" && grabbed?.Key == "beer")
        {
            // NOTE: We use child nodes with small gdscript behaviours attached
            // to handle this sort of cosmetic stuff in a dynamic way.
            switched.GetNode("PlantScript").Call("cover_in_beer");
            grabbed.GetNode("BeerScript").Call("empty_bottle");

            sentence = "Regué la planta con cerveza";
        }

        // More special actions here...

        if (sentence == "")
        {
            sentence = $"Toqué {switched.Prop.HumanReadableName}";
        }

        GD.Print(sentence);
        EmitSignal(nameof(NewJournalAction), sentence);
    }
}
