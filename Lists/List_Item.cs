using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

public enum ItemType
{
    None,
    Weapon,
    Armour,
    Consumable,
    Unknown
}

public enum WeaponType
{
    None,
    OneHanded,
    TwoHanded
}

public enum WeaponClass
{
    None,
    Axe,
    ShortSword,
    Spear
}

public enum ArmourType
{
    None,
    Chest,
    Head,
    Legs,
    Feet
}
public abstract class List_Item
{
    private static HashSet<int> usedIDs = new HashSet<int>();

    // General
    public int itemID;
    public ItemType itemType;
    public WeaponType weaponType;
    public WeaponClass weaponClass;
    public ArmourType armourType;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize;
    public int itemValue;
    public float itemWeight;
    public bool equippable;
    public Vector3 itemScale;
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public AnimatorController itemAnimatorController;

    // Attack
    public float itemDamage;
    public float itemSpeed;
    public float itemForce;
    public float itemRange;

    // Defence
    public float itemMaxHealthBonus;
    public float itemPhysicalArmour;
    public float itemMagicalArmour;
    public float itemCoverage;

    public virtual void Start()
    {
       
    }

    public static void AddToList(List<List_Item> list, List_Item item)
    {
        if (usedIDs.Contains(item.itemID))
        {
            throw new ArgumentException("Item ID " + item.itemID + " is already used");
        }

        usedIDs.Add(item.itemID);
        list.Add(item);
    }

    public static List_Item GetItemData(int itemID)
    {
        List<List_Item> targetList;

        switch (itemID)
        {
            case 1:
                targetList = List_Weapon.allWeaponData;
                break;
            case 2:
                targetList = List_Armour.allArmourData;
                break;
            case 3:
                targetList = List_Consumable.allConsumableData;
                break;
            default:
                return null;
        }

        return SearchItemInList(itemID, targetList);
    }

    private static List_Item SearchItemInList(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.itemID == itemID)
            {
                return data;
            }
        }

        return null;
    }

    public int GetMaxStackSize()
    {
        return maxStackSize;
    }

    public static Sprite GetItemSprite(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.itemID == itemID)
            {
                return data.itemIcon;
            }
        }

        return null;
    }

    [Serializable]
    public struct ItemStats
    {
        public CommonStats CommonStats;
        public WeaponStats WeaponStats;
        public ArmourStats ArmourStats;
    }

    [Serializable]
    public struct CommonStats
    {
        public int itemID;
        public ItemType itemType;
        public string itemName;
        public Sprite itemIcon;
        public int maxStackSize;
        public int currentStackSize;
        public int itemValue;
        public float itemWeight;
        public bool equippable;
        public Vector3 itemScale;
        public Vector3 itemPosition;
        public Vector3 itemRotation;
        public AnimatorController itemAnimatorController;
    }

    [Serializable]
    public struct WeaponStats
    {
        public WeaponType weaponType;
        public WeaponClass weaponClass;
        public float itemDamage;
        public float itemSpeed;
        public float itemForce;
        public float itemRange;
    }

    [Serializable]
    public struct ArmourStats
    {
        public ArmourType armourType;
        public float itemMaxHealthBonus;
        public float itemPhysicalArmour;
        public float itemMagicalArmour;
        public float itemCoverage;
    }

    public static ItemStats DisplayItemStats(int itemID, int currentStackSize = -1)
    {
        List_Item item = GetItemData(itemID);

        if (item != null)
        {
            ItemStats displayItem = new ItemStats()
            {
                CommonStats = new CommonStats()
                {
                    itemID = item.itemID,
                    itemType = item.itemType,
                    itemName = item.itemName,
                    itemIcon = item.itemIcon,
                    currentStackSize = currentStackSize,
                    maxStackSize = item.maxStackSize,
                    itemValue = item.itemValue,
                    itemWeight = item.itemWeight,
                    equippable = item.equippable,
                },

                WeaponStats = new WeaponStats()
                {
                    weaponType = item.weaponType,
                    weaponClass = item.weaponClass,
                    itemDamage = item.itemDamage,
                    itemSpeed = item.itemSpeed,
                    itemForce = item.itemForce,
                    itemRange = item.itemRange,
                },
                
                ArmourStats = new ArmourStats()
                {
                    armourType = item.armourType,
                    itemMaxHealthBonus = item.itemMaxHealthBonus,
                    itemPhysicalArmour = item.itemPhysicalArmour,
                    itemMagicalArmour = item.itemMagicalArmour,
                    itemCoverage = item.itemCoverage
                }
            };
                return displayItem;
        }

        else
        {
            Debug.Log($"Failed to retrieve item stats for itemID: {itemID}");
            return new ItemStats
            {
                CommonStats = new CommonStats
                {
                    itemType = ItemType.Unknown
                }
            };
        }
    }
}