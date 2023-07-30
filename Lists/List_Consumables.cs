using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    HealthPotion,
    ManaPotion,
    StaminaPotion
}

public class List_Consumable : List_Item
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

    public List_Consumable
        (int itemID, 
        ConsumableType consumableType, 
        string consName, 
        int consValue, 
        Sprite consIcon, 
        int maxStackSize)
    {
        this.itemID = itemID;
        this.consumableType = consumableType;
        this.consName = consName;
        this.consValue = consValue;
        this.consIcon = consIcon;
        this.maxStackSize = maxStackSize;
    }

    public static void InitializeConsumableData()
    {
        Potions();
    }

    static void Potions()
    {
        Sprite test_spr = Resources.Load<Sprite>("Assets/Artwork/Assets/0_Assets/Atlas/at_dungeon_01/obj_wep_m_ss_01");
        List_Consumable potions = new List_Consumable
            (2, 
            ConsumableType.HealthPotion, 
            "Health Potion", 
            100, 
            test_spr, 
            99);

        AddToList(allConsumableData, potions);
    }
}
