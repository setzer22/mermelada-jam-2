using Godot;
using System;

public class InitGame : Node
{
    
    /// <summary>
    /// If set to a value other than -1, will jump to that level instead of the
    /// start of the game. Levels start at 0!
    /// </summary>
    [Export]
    public int SkipToLevel = -1;
    
    // The only purpose of this class is to launch the main menu of the game via
    // the game manager. This is the scene that the game gets loaded from
    public override async void _Ready()
    {
        await ToSignal(GetTree(), "idle_frame"); 

        if (SkipToLevel != -1) {
            GameManager.Singleton.levelIndex = SkipToLevel - 1;
            GameManager.Singleton.GoToNextLevel();
        } else {
            GameManager.Singleton.GoToMainMenu();
        }
    }
}
