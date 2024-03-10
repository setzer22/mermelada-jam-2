using Godot;
using System;

public class InitGame : Node
{
    // The only purpose of this class is to launch the main menu of the game via
    // the game manager. This is the scene that the game gets loaded from
    public override void _Ready()
    {
        GameManager.Singleton.GoToMainMenu();
    }
}
