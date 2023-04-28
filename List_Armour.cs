using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Armour : Manager_Item
{
    public static List<List_Armour> allArmourData;

    public List_Armour(int itemID, string itemName, Sprite itemIcon)
    {
        this.itemID = itemID;
        this.itemName = itemName;
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
