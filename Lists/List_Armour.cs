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
        Vector3 itemPosition,
        Vector3 itemRotation,
        Vector3 itemScale,
        bool equippable,
        //AnimatorController itemAnimatorController,
        int maxStackSize,
        int itemValue,
        float itemMaxHealthBonus,
        float itemPhysicalArmour,
        float itemMagicalArmour,
        float itemCoverage,
        float itemWeight)
    {
        this.ItemStats.CommonStats.ItemID = itemID;
        this.ItemStats.CommonStats.ItemType = itemType;
        this.ItemStats.ArmourStats.ArmourType = armourType;
        this.ItemStats.CommonStats.ItemName = itemName;
        this.ItemStats.CommonStats.ItemIcon = itemIcon;
        this.ItemStats.CommonStats.ItemPosition = itemPosition;
        this.ItemStats.CommonStats.ItemRotation = itemRotation;
        this.ItemStats.CommonStats.ItemScale = itemScale;
        this.ItemStats.CommonStats.Equippable = equippable;
        //this.itemAnimatorController = itemAnimatorController;
        this.ItemStats.CommonStats.MaxStackSize = maxStackSize;
        this.ItemStats.CommonStats.ItemValue = itemValue;
        this.ItemStats.ArmourStats.ItemMaxHealthBonus = itemMaxHealthBonus;
        this.ItemStats.ArmourStats.ItemPhysicalArmour = itemPhysicalArmour;
        this.ItemStats.ArmourStats.ItemMagicalArmour = itemMagicalArmour;
        this.ItemStats.ArmourStats.ItemCoverage = itemCoverage;
        this.ItemStats.CommonStats.ItemWeight = itemWeight;
    }

    static void Heavy()
    {
        List_Armour bronzeChestplate01 = new List_Armour(
            2,
            ItemType.Armour,
            ArmourType.Chest,
            "Bronze chestplate",
            SO_List.Instance.ArmourSprites[0].sprite,
            new Vector3(-0.04f, -0.07f, 0f),
            new Vector3(180, 0, 0),
            new Vector3(0.4f, 0.4f, 0.4f),
            true,
            //SO_List.Instance.animatorControllers[0].animatorController,
            1,
            10,
            5,
            2,
            2,
            75,
            10);

        AddToList(allArmourData, bronzeChestplate01);
    }
}
