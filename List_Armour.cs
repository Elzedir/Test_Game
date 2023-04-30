using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Armour : Manager_Item
{
    public static List<List_Armour> allArmourData;

    public ArmourType armourType;
    public enum ArmourType
    {
        Chest,
        Head,
        Legs,
        Feet
    }

    public List_Armour(int itemID, ItemType itemType, ArmourType armourType, string itemName, Sprite itemIcon)
    {
        this.itemID = itemID;
        this.itemType = itemType;
        this.armourType = armourType;
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
