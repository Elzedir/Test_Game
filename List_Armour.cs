using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Armour : List_Item
{
    public static List<List_Armour> allArmourData;

    public List_Armour(int itemID, string itemName, string itemType, Sprite itemIcon): base(itemID, itemName)
    {
        List_Item.itemID = itemID;
        this.itemName = itemName;
        this.itemType = itemType;
        this.itemIcon = itemIcon;
    }

    public static void InitializeArmourData()
    {
        Heavy();
    }

    static void Heavy()
    {

    }
}
