using Godot;
using System;
using System.Collections.Generic;

public class SceneInventoryData //:Node
{
    //[Signal] public delegate void PlayerInventoryChangedEventHandler();
    public event Action OnInventoryChanged = delegate { };

    public static SceneInventoryData Instance
    {
        get => instance;
        private set => throw new NotImplementedException();
    }

    static readonly SceneInventoryData instance = new SceneInventoryData();
    readonly List<IInteractiveProp> props = new List<IInteractiveProp>();

    public IInteractiveProp[] GetPropInventory() => props.ToArray();

    public void Clear()
    {
        props.Clear();
    }

    public void AddProp(IInteractiveProp prop)
    {
        props.Add(prop);
        GD.Print($"object added to scene: {prop.Name}");
        //EmitSignal(PlayerInventoryChanged);
        OnInventoryChanged();
    }

    public void RemoveProp(IInteractiveProp prop)
    {
        props.Remove(prop);
        //EmitSignal(PlayerInventoryChanged);
        OnInventoryChanged();
    }
}