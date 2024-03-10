using Godot;
using System;

public interface IInteractiveProp
{
    event Action<InteractionLevel> InteractionEmitted;

    string Name { get; }

    void Interact();
}

public enum InteractionLevel : uint
{
    cosmetic = default,
    level1 = 1,
    level2 = 2,
    level3 = 3,
}

public static class InteractionExtensions
{
    public static bool TryGetProp(this Area2D area, out IInteractiveProp prop)
    {
        prop = area.GetNode<IInteractiveProp>("InteractionArea");

        return prop != null;
    }

    public static bool TryGetPlayer(this KinematicBody2D area, out Player player)
    {
        player = area as Player;

        return player != null;
    }
}