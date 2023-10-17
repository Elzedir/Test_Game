using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArmourType
{
    None,
    Chest,
    Head,
    Legs,
    Feet
}

public class List_Item_Armour : List_Item
{
    public static List<List_Item> AllArmourData = new();

    public static void InitializeArmourData()
    {
        Heavy();

        foreach (var armour in AllArmourData)
        {
            armour.Start();
        }
    }

    public List_Item_Armour (ItemStats itemStats) { this.ItemStats = itemStats; }

    static void Heavy()
    {
        CommonStats commonStats = new CommonStats(
            itemID: 2,
            itemType: ItemType.Armour,
            itemName: "Bronze ChestPlate",
            itemIcon: List_Item.GetSpriteFromSpriteSheet(path: "Resources_Sprite/Armour/armours", name: "armours_0"),
            itemPosition: new Vector3(-0.04f, -0.07f, 0f),
            itemRotation: new Vector3(180, 0, 0),
            itemScale: new Vector3(0.4f, 0.4f, 0.4f),
            itemEquippable: true,
            maxStackSize: 1,
            itemValue: 10
            );

        ArmourStats armourStats = new ArmourStats(
            armourType: ArmourType.Chest,
            itemCoverage: 75
            );

        FixedModifiers fixedModifiers = new FixedModifiers(
            maxHealth: 5,
            maxMana: 5,
            maxStamina: 5,
            pushRecovery: 1,
            physicalDefence: 2,
            magicalDefence: 2,
            dodgeCooldownReduction: -1
            );

        PercentageModifiers percentageModifiers = new PercentageModifiers(
            attackSpeed: 0.92f
            );

        ItemStats bronzeChestPlate01 = new ItemStats(commonStats: commonStats, armourStats: armourStats, fixedModifiers: fixedModifiers, percentageModifiers: percentageModifiers);

        List_Item_Weapon bronzeChestPlate01Weapon = new List_Item_Weapon(bronzeChestPlate01);

        AddToList(AllArmourData, bronzeChestPlate01Weapon);
    }
}

[Serializable]
public class ArmourStats
{
    public ArmourType ArmourType;
    public float ItemCoverage;

    public ArmourStats(
        ArmourType armourType = ArmourType.None,
        float itemCoverage = 0
        )
    {
        this.ArmourType = armourType;
        this.ItemCoverage = itemCoverage;
    }
}
