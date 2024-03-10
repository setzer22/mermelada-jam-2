using System;
using System.Collections.Generic;

public class PlayerInventoryData //:Node
{
    //[Signal] public delegate void PlayerInventoryChangedEventHandler();
    public event Action OnInventoryChanged = delegate { };

    public static PlayerInventoryData Instance
    {
        get => instance;
        private set => throw new NotImplementedException();
    }

    static readonly PlayerInventoryData instance = new PlayerInventoryData();
    readonly List<IInteractiveProp> props = new List<IInteractiveProp>();

    public IInteractiveProp[] GetPropInventory() => props.ToArray();

    public void AddProp(IInteractiveProp prop)
    {
        props.Add(prop);
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