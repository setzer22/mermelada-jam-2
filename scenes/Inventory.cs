using Godot;
using System;
using System.Collections.Generic;

public class ItemData {
    public string ItemName;
    public Texture ItemTexture;
}

public class Inventory : Node2D
{
    private List<Item> items = new List<Item>();

    private static Dictionary<string, ItemData> ItemDB = new Dictionary<string, ItemData>();

    public override void _EnterTree()
    {
        base._EnterTree();
        if (ItemDB.Count == 0) {
            ItemDB.Add("tenedor", new ItemData {
                ItemName = "Tenedor",
                ItemTexture = GD.Load<Texture>("res://resources/sprites/6irokKLXT.png")
            });
            ItemDB.Add("cerveza", new ItemData {
                ItemName = "Cerveza",
                ItemTexture = GD.Load<Texture>("res://resources/sprites/beer-svgrepo-com.svg")
            });
            ItemDB.Add("pinturas", new ItemData {
                ItemName = "Pinturas",
                ItemTexture = GD.Load<Texture>("res://resources/sprites/watercolor-watercolor-svgrepo-com.svg")
            });
            ItemDB.Add("cartera", new ItemData {
                ItemName = "Cartera",
                ItemTexture = GD.Load<Texture>("res://resources/sprites/wallet-svgrepo-com.svg")
            });
            ItemDB.Add("movil", new ItemData {
                ItemName = "Movil",
                ItemTexture = GD.Load<Texture>("res://resources/sprites/smartphone-svgrepo-com.svg")
            });
        }
    }



    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public List<Item> GetItems()
    {
        return items;
    }
}
