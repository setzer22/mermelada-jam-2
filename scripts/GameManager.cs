using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public class GameManager : Node
{
    public static GameManager Singleton;

    const int MAX_ACTIONS = 2;

    [Signal]
    delegate void InkSpent(float amountPct);

    [Signal]
    delegate void TenantDamaged(float amountPct);

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

        int annoyance = LevelLogic.GetAnnoyanceLevel(levelIndex, item, surface);
        HandlePerformedActionLogic(sentence, annoyance);
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

        int annoyance = LevelLogic.GetAnnoyanceLevel(levelIndex, grabbed, switched);
        HandlePerformedActionLogic(sentence, annoyance);
    }

    public void HandlePerformedActionLogic(string sentence, int annoyance)
    {
        if (performedActions.Add(sentence))
        {
            accumulatedAnnoyanceLevel += annoyance;

            var maxAnnoyance = LevelLogic.GetMaxAnnoyanceLevel(levelIndex);
            var maxInk = LevelLogic.GetMaxInkLevel(levelIndex);

            GD.Print(
                $"Annoyance: {annoyance}. Max annoyance: {maxAnnoyance}. Accumulated annoyance: {accumulatedAnnoyanceLevel}"
            );

            EmitSignal(nameof(InkSpent), (1f / (float)maxInk) * 100f);
            EmitSignal(nameof(TenantDamaged), (annoyance / (float)maxAnnoyance) * 100f);
            EmitSignal(nameof(NewJournalAction), sentence);
            EmitSignal(nameof(UpdateRatings), annoyance);

            GD.PrintErr(accumulatedAnnoyanceLevel);
            if (performedActions.Count > maxAnnoyance)
            {
                RestartLevel();
            }
            else if (accumulatedAnnoyanceLevel >= LevelLogic.GetMaxAnnoyanceLevel(levelIndex))
            {
                // We wait so that the screen doesn't show up too fast.
                WaitAndGoToNextLevel();
            }
        }
    }

    public async void WaitAndGoToNextLevel()
    {
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GoToNextLevel();
    }

    private readonly HashSet<string> performedActions = new HashSet<string>();
    private int accumulatedAnnoyanceLevel;
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
        PackedScene levelScene,
        PackedScene tenant
    )
    {
        currentLevel?.QueueFree();
        performedActions.Clear();
        accumulatedAnnoyanceLevel = 0;

        foreach (KeyValuePair<PackedScene, bool> pair in transitionScenes)
        {
            await PlayTransition(pair.Key, pair.Value);
        }

        var level = levelScene.Instance();
        GetTree().Root.AddChild(level);
        level.AddChild(tenant.Instance());
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
            levelScene: currentLevelScene,
            tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
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
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn"),
                    tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
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
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn"),
                    tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
                );
                break;
            case 2:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/PainterIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/2_ToRent.tscn"), false },
                        {
                            GD.Load<PackedScene>("res://scenes/Tenants/BlueCollarIntro.tscn"),
                            false
                        },
                    },
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn"),
                    tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
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
                    levelScene: GD.Load<PackedScene>("res://scenes/Main.tscn"),
                    tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
                );
                break;
            case 4:
                LoadLevel(
                    new Dictionary<PackedScene, bool>()
                    {
                        { GD.Load<PackedScene>("res://scenes/Tenants/InvestorIntro.tscn"), true },
                        { GD.Load<PackedScene>("res://scenes/ToRent/4_ToRent.tscn"), false },
                        {
                            GD.Load<PackedScene>("res://scenes/Tenants/GhostHunterIntro.tscn"),
                            false
                        },
                    },
                    GD.Load<PackedScene>("res://scenes/Main.tscn"),
                    tenant: GD.Load<PackedScene>("res://scenes/Tenants/Posh.tscn")
                );
                break;
            case 5:
                LoadEndGame(GD.Load<PackedScene>("res://scenes/Tenants/GhostHunterIntro.tscn"));
                break;
        }
    }
}
