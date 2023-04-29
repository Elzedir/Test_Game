using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;


public class Manager_Item
{
    public static Manager_Item instance;

    public static Dictionary<int, object> itemDictionary;

    private static HashSet<int> usedIDs = new HashSet<int>();

    // General
    public int maxStackSize;
    public int itemID;
    public ItemType itemType;
    public enum ItemType
    {
        Weapon,
        Armour,
        Consumable
    }

    public string itemName;
    public float itemValue;
    public Sprite itemIcon;

    //Damage
    public float itemDamage;
    public float itemSpeed;
    public float itemForce;
    public float itemRange;

    //Defence

    // States
    public bool equipabble = false; // Later on, simply use layers to check whether some things are equipable or not

    public virtual void Awake()
    {
        instance = this;

        itemDictionary = new Dictionary<int, object>();

        List_Weapon.allWeaponData = new List<List_Weapon>();
        List_Weapon.InitializeWeaponData();
        List_Armour.allArmourData = new List<List_Armour>();
        List_Armour.InitializeArmourData();
        List_Consumables.allConsumableData = new List<List_Consumables>();
        List_Consumables.InitializeConsumableData();

        foreach (List_Weapon weaponData in List_Weapon.allWeaponData)
        {
            itemDictionary.Add(itemID, weaponData);
        }

        foreach (List_Armour armourData in  List_Armour.allArmourData)
        {
            itemDictionary.Add(itemID, armourData);
        }

        foreach (List_Consumables consumableData in List_Consumables.allConsumableData)
        {
            itemDictionary.Add(itemID, consumableData);
        }
    }

    public static void AddToList<T>(List<T> list, T item) where T : Manager_Item
    {
        if (usedIDs.Contains(item.itemID))
        {
            throw new ArgumentException("Item ID " + item.itemID + " is already used");
        }

        usedIDs.Add(item.itemID);
        list.Add(item);
    }

    public static object GetItemData(int itemID)
    {
        if (itemDictionary.ContainsKey(itemID))
        {
            object itemData = itemDictionary[itemID];
            switch (itemData)
            {
                case List_Weapon weaponData:
                        return weaponData;
                case List_Armour armourData:
                        return armourData;
                case List_Consumables consumableData:
                        return consumableData;
                default:
                    Debug.Log("Invalid item type");
                    break;
            }
        }

        else
        {
            Debug.LogError("Item ID " + itemID + " does not exist.");
        }

        return null;
    }
}
