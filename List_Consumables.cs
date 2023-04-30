using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Consumable : Manager_Item
{
    public static List<List_Consumable> allConsumableData;

    public ConsumableType consumableType;
    public enum ConsumableType
    {
        HealthPotion,
        ManaPotion,
        StaminaPotion
    }

    public List_Consumable(int itemID, ItemType itemType, ConsumableType consumableType, string itemName, float consValue, Sprite consIcon)
    {
        this.itemID = itemID;
        this.itemType = itemType;
        this.consumableType = consumableType;
        this.itemName = itemName;
        this.itemValue = consValue;
        this.itemIcon = consIcon;
    }

    public static void InitializeConsumableData()
    {
        Potions();
    }

    static void Potions()
    {

    }
}
