using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Consumables : List_Item
{
    public static List<List_Consumables> allConsumableData;

    public List_Consumables(int itemID, string itemName, string itemType, float consValue, Sprite consIcon) : base(itemID, itemName)
    {
        List_Item.itemID = itemID;
        this.itemName = itemName;
        this.itemType = itemType;
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
