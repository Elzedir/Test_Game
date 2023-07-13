using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class List_Armour : List_Item
{
    public static List<List_Item> allArmourData = new();

    public static void InitializeArmourData()
    {
        Heavy();

        foreach (var armour in allArmourData)
        {
            armour.Start();
        }
    }

    public List_Armour
        (int itemID,
        ItemType itemType,
        ArmourType armourType,
        string itemName,
        Sprite itemIcon,
        Vector3 itemScale,
        Vector3 itemPosition,
        Vector3 itemRotation,
        //AnimatorController itemAnimatorController,
        int maxStackSize,
        int itemValue,
        float itemMaxHealthBonus,
        float itemPhysicalArmour,
        float itemMagicalArmour,
        float itemCoverage,
        float itemWeight)
    {
        this.itemID = itemID;
        this.itemType = itemType;
        this.armourType = armourType;
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.itemScale = itemScale;
        this.itemPosition = itemPosition;
        this.itemRotation = itemRotation;
        //this.itemAnimatorController = itemAnimatorController;
        this.maxStackSize = maxStackSize;
        this.itemValue = itemValue;
        this.itemMaxHealthBonus = itemMaxHealthBonus;
        this.itemPhysicalArmour = itemPhysicalArmour;
        this.itemMagicalArmour = itemMagicalArmour;
        this.itemCoverage = itemCoverage;
        this.itemWeight = itemWeight;
    }

    static void Heavy()
    {
        List_Armour heavyData = new List_Armour(
            2,
            ItemType.Armour,
            ArmourType.Chest,
            "Bronze chestplate",
            SO_List.Instance.armourSprites[0].sprite,
            new Vector3(0.4f, 0.4f, 0.4f),
            new Vector3(0f, 0f, 0f),
            new Vector3(0, 0, 0),
            //SO_List.Instance.animatorControllers[0].animatorController,
            1,
            10,
            5,
            2,
            2,
            75,
            10);

        AddToList(allArmourData, heavyData);
    }
}
