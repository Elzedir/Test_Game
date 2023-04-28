using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Consumables : Manager_Item
{
    public static List<List_Consumables> allConsumableData;

    public List_Consumables(int itemID, string itemName, float consValue, Sprite consIcon)
    {
        this.itemID = itemID;
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
