using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    HealthPotion,
    ManaPotion,
    StaminaPotion
}

public class List_Item_Consumable : List_Item
{
    public static List<List_Item> allConsumableData = new List<List_Item>();

    public ConsumableType consumableType;
    
    public string consName;
    public int consValue;
    public Sprite consIcon;

    public override void Start()
    {
        allConsumableData = new List<List_Item>();
        InitializeConsumableData();
    }

    public List_Item_Consumable(ItemStats itemStats) { this.ItemStats = itemStats; }

    public static void InitializeConsumableData()
    {
        Potions();
    }

    static void Potions()
    {
        
    }
}
