using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Armour : List_Item
{
    public static List<List_Item> allArmourData = new List<List_Item>();

    public override void Start()
    {
        List_Armour.allArmourData = new List<List_Item>();
        List_Armour.InitializeArmourData();
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
