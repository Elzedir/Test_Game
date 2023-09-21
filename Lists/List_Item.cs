using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.Progress;

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
    OneHandedMelee,
    TwoHandedMelee,
    OneHandedRanged,
    TwoHandedRanged,
    OneHandedMagic,
    TwoHandedMagic
}

public enum WeaponClass
{
    None,
    Axe,
    Shortbow,
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
    private static HashSet<int> _usedIDs = new HashSet<int>();
    public ItemStats ItemStats;

    public virtual void Start()
    {
       
    }

    public static void AddToList(List<List_Item> list, List_Item item)
    {
        if (_usedIDs.Contains(item.ItemStats.CommonStats.ItemID))
        {
            throw new ArgumentException("Item ID " + item.ItemStats.CommonStats.ItemID + " is already used");
        }

        _usedIDs.Add(item.ItemStats.CommonStats.ItemID);
        list.Add(item);
    }

    public static List_Item GetItemData(int itemID)
    {
        List<List_Item>[] allLists = new List<List_Item>[]
        {
        List_Item_Weapon.AllWeaponData,
        List_Item_Armour.allArmourData,
        List_Item_Consumable.allConsumableData
        };

        foreach (var list in allLists)
        {
            List_Item foundItem = SearchItemInList(itemID, list);
            if (foundItem != null)
            {
                return foundItem;
            }
        }
        return null;
    }

    private static List_Item SearchItemInList(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.ItemStats.CommonStats.ItemID == itemID)
            {
                return data;
            }
        }

        return null;
    }

    public static Sprite GetItemSprite(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.ItemStats.CommonStats.ItemID == itemID)
            {
                return data.ItemStats.CommonStats.ItemIcon;
            }
        }

        return null;
    }

    public void AttachWeaponScript(List_Item item, Equipment_Slot equipmentSlot)
    {
        GameManager.Destroy(equipmentSlot.GetComponent<Weapon>());

        foreach (WeaponType weaponType in item.ItemStats.WeaponStats.WeaponType)
        {
            switch (weaponType)
            {
                case WeaponType.OneHandedMelee:
                case WeaponType.TwoHandedMelee:
                    foreach (WeaponClass weaponClass in item.ItemStats.WeaponStats.WeaponClass)
                    {
                        switch (weaponClass)
                        {
                            case WeaponClass.Axe:
                                //equipmentSlot.AddComponent<Weapon_Axe>();
                                break;
                            case WeaponClass.ShortSword:
                                //equipmentSlot.AddComponent<Weapon_ShortSword>();
                                break;
                                // Add more cases here
                        }
                    }
                    break;
                case WeaponType.OneHandedRanged:
                case WeaponType.TwoHandedRanged:
                    equipmentSlot.AddComponent<Weapon_Bow>();
                    break;
                case WeaponType.OneHandedMagic:
                case WeaponType.TwoHandedMagic:
                    foreach (WeaponClass weaponClass in item.ItemStats.WeaponStats.WeaponClass)
                    {
                        //switch (weaponClass)
                        //{
                        //    case WeaponClass.Staff:
                        //        equipmentSlot.AddComponent<Weapon_Staff>();
                        //        break;
                        //    case WeaponClass.Wand:
                        //        equipmentSlot.AddComponent<Weapon_Wand>();
                        //        break;
                        //         Add more cases here
                        //}
                    }
                    break;
            }
        }
    }
}

[Serializable]
public struct ItemStats
{
    public CommonStats CommonStats;
    public CombatStats CombatStats;
    public WeaponStats WeaponStats;
    public ArmourStats ArmourStats;

    public static ItemStats SetItemStats(int itemID, int stackSize)
    {
        if (itemID != -1 && stackSize != 0)
        {
            ItemStats newItemStats = List_Item.GetItemData(itemID).ItemStats;
            newItemStats.CommonStats.CurrentStackSize = stackSize;
            return newItemStats;
        }
        else
        {
            return ItemStats.None();
        }
    }

    public static ItemStats None()
    {
        ItemStats none = new ItemStats();
        none.CommonStats.ItemID = -1;
        none.CommonStats.CurrentStackSize = 0;
        return none;
    }
}

[Serializable]
public struct CommonStats
{
    public int ItemID;
    public ItemType ItemType;
    public string ItemName;
    public Sprite ItemIcon;
    public int MaxStackSize;
    public int CurrentStackSize;
    public int ItemValue;
    public float ItemWeight;
    public bool Equippable;
    public Vector3 ItemScale;
    public Vector3 ItemPosition;
    public Vector3 ItemRotation;
    public AnimatorController ItemAnimatorController;
}

[Serializable]
public struct WeaponStats
{
    public WeaponType[] WeaponType;
    public WeaponClass[] WeaponClass;
    public float MaxChargeTime;
}

[Serializable]
public struct ArmourStats
{
    public ArmourType ArmourType;
    public float ItemCoverage;
}