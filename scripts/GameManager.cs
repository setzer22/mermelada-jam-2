using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class GameManager : Node
{
    public static GameManager Singleton;

    const int MAX_ACTIONS = 2;

    [Signal]
    delegate void InkSpent(float amount);

    [Signal]
    delegate void NewJournalAction(string sentence);

    [Signal]
    delegate void UpdateRatings(int numBadRatings);

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

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("CheatNextLevel"))
        {
            GoToNextLevel();
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

        if (performedActions.Add(sentence))
        {
            EmitSignal(nameof(NewJournalAction), sentence);

            if (performedActions.Count > MAX_ACTIONS)
            {
                RestartLevel();
            }
        }
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

        if (performedActions.Add(sentence))
        {
            EmitSignal(nameof(NewJournalAction), sentence);

            if (performedActions.Count > MAX_ACTIONS)
            {
                RestartLevel();
            }
        }
    }

    private readonly HashSet<string> performedActions = new HashSet<string>();
    public PackedScene currentLevelScene = null;
    public Node currentLevel = null;

    public void GoToMainMenu()
    {
        currentLevel?.QueueFree();

        var mainMenuScene = GD.Load<PackedScene>("res://scenes/MainMenu.tscn");
        var mainMenu = mainMenuScene.Instance();
        AddChild(mainMenu);

        currentLevel = mainMenu;
    }

    public async Task PlayTransition(PackedScene outroScene, bool isOutro)
    {
        var outro =
            outroScene.Instance() as LevelIntro
            ?? throw new Exception("Outro scene must be a LevelIntro");
        outro.IsOutro = isOutro;

        GetTree().Root.AddChild(outro);

        await ToSignal(outro, "Finished");

        outro.QueueFree();
    }

    public async void LoadLevel(
       Dictionary<PackedScene, bool> transitionScenes,
        PackedScene levelScene
    )
    {
        currentLevel?.QueueFree();
        performedActions.Clear();

        foreach (KeyValuePair<PackedScene, bool> pair in transitionScenes)
        {
            await PlayTransition(pair.Key, pair.Value);
        }

        var level = levelScene.Instance();
        GetTree().Root.AddChild(level);
        currentLevel = level;
        currentLevelScene = levelScene;
    }

    public async void LoadEndGame(PackedScene outroScene)
    {
        currentLevel?.QueueFree();

        await PlayTransition(outroScene, true);

        var endGame = GD.Load<PackedScene>("res://scenes/EndGame.tscn").Instance();
        GetTree().Root.AddChild(endGame);
        currentLevel = endGame;
    }

    public int levelIndex = -1;

    public void RestartLevel()
    {
        LoadLevel(
            new Dictionary<PackedScene, bool>()
            {
                { GD.Load<PackedScene>("res://scenes/Player/GameOver.tscn"), true },
                { GD.Load<PackedScene>("res://scenes/Player/Retry.tscn"), false },
            },
            levelScene: currentLevelScene
        );
    }

    public void GoToNextLevel()
    {
        levelIndex += 1;

        switch (levelIndex)
        {
            // TODO: !!!! ADJUST MAIN.tscn to load different levels
            case 0:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Player/GameIntro.tscn"), false },
                        { GD.Load<PackedScene>("res://scenes/ToRent/0_ToRent.tscn"), false },
                        { GD.Load<PackedScene>("res://scenes/Tenants/PoshIntro.tscn"), false },
                    },
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn")
                );
                break;
            case 1:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/PoshIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/1_ToRent.tscn"), false },
                        { GD.Load<PackedScene>("res://scenes/Tenants/PainterIntro.tscn"), false },
                    },
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn")
                );
                break;
            case 2:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/PainterIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/2_ToRent.tscn"), false },
                        {  GD.Load<PackedScene>("res://scenes/Tenants/BlueCollarIntro.tscn"), false },
                    },
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn")
                );
                break;
            case 3:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/BlueCollarIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/3_ToRent.tscn"), false },
                        { GD.Load<PackedScene>("res://scenes/Tenants/InvestorIntro.tscn"), false },
                    },
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn")
                );
                break;
            case 4:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/InvestorIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/4_ToRent.tscn"), false },
                        { GD.Load<PackedScene>("res://scenes/Tenants/GhostHunterIntro.tscn"), false },
                    },
                    GD.Load<PackedScene>("res://scenes/Main.tscn")
                );
                break;
            case 5:
                LoadEndGame(
                    GD.Load<PackedScene>("res://scenes/Tenants/GhostHunterIntro.tscn")
                );
                break;
        }
    }
}
