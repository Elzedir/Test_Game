using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmourType
{
    Chest,
    Head,
    Legs,
    Feet
}

public class List_Armour : List_Item
{
    public static List<List_Item> allArmourData = new List<List_Item>();

    public ArmourType armourType;

    public string armourName;
    public Sprite armourIcon;
    public float healthBonus;
    public float physicalDefence;
    public float magicalDefence;

    public override void Start()
    {
        List_Armour.allArmourData = new List<List_Item>();
        List_Armour.InitializeArmourData();
    }

    public List_Armour(int itemID, ArmourType armourType, string armourName, Sprite armourIcon)
    {
        this.itemID = itemID;
        this.armourType = armourType;
        this.armourName = armourName;
        this.armourIcon = armourIcon;
    }

    public static void InitializeArmourData()
    {
        Heavy();
    }

    static void Heavy()
    {

    }
}
