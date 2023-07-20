using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Animations;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armour,
    Consumable,
    Unknown
}

public enum WeaponType
{
    OneHanded,
    TwoHanded
}

public enum WeaponClass
{
    Axe,
    ShortSword,
    Spear
}

public enum ArmourType
{
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

        // Attack
        public WeaponType weaponType;
        public WeaponClass weaponClass;
        public float itemDamage;
        public float itemSpeed;
        public float itemForce;
        public float itemRange;

        // Defence
        public ArmourType armourType;
        public float itemMaxHealthBonus;
        public float itemPhysicalArmour;
        public float itemMagicalArmour;
        public float itemCoverage;
    }

    public static ItemStats DisplayItemStats(int itemID, int currentStackSize = -1)
    {
        Debug.Log("1");
        List_Item item = GetItemData(itemID);

        if (item != null)
        {
            ItemStats displayItem = new ItemStats()
            {
                itemID = item.itemID,
                itemType = item.itemType,
                itemName = item.itemName,
                itemIcon = item.itemIcon,
                currentStackSize = currentStackSize,
                maxStackSize = item.maxStackSize,
                itemValue = item.itemValue, 
                itemWeight = item.itemWeight,
                equippable = item.equippable
            };

            switch (item.itemType)
            {
                case ItemType.Weapon:
                    displayItem.weaponType = item.weaponType;
                    displayItem.weaponClass = item.weaponClass;
                    displayItem.itemDamage = item.itemDamage;
                    displayItem.itemSpeed = item.itemSpeed;
                    displayItem.itemForce = item.itemForce;
                    displayItem.itemRange = item.itemRange;
                    break;
                case ItemType.Armour:
                    displayItem.armourType = item.armourType;
                    displayItem.itemMaxHealthBonus = item.itemMaxHealthBonus;
                    displayItem.itemPhysicalArmour = item.itemPhysicalArmour;
                    displayItem.itemMagicalArmour = item.itemMagicalArmour;
                    displayItem.itemCoverage = item.itemCoverage;
                    break;
                case ItemType.Consumable:
                    // ... consumable-specific assignments ...
                    break;
                default:
                    Debug.Log($"Unknown item type for itemID: {itemID}");
                    break;
            }

            return displayItem;
        }
        else
        {
            Debug.Log($"Failed to retrieve item stats for itemID: {itemID}");
            return new ItemStats { itemType = ItemType.Unknown };
        }
    }
}