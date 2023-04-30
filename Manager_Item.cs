using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;


public enum ItemType
{
    Weapon,
    Armour,
    Consumable
}
public class Manager_Item : MonoBehaviour
{
    public static Manager_Item instance;

    public static Dictionary<int, Manager_Item> itemDictionary;

    private static HashSet<int> usedIDs = new HashSet<int>();

    // General
    public int maxStackSize;
    public int itemID;
    public ItemType itemType;
    public string itemName;
    public float itemValue;
    public Sprite itemIcon;
    public float itemDamage;
    public float itemSpeed;
    public float itemForce;
    public float itemRange;


    //Defence

    // States
    public bool equipabble = false; // Later on, simply use layers to check whether some things are equipable or not

    public virtual void Start()
    {
        instance = this;

        itemDictionary = new Dictionary<int, Manager_Item>();

        List_Weapon.allWeaponData = new List<List_Weapon>();
        List_Weapon.InitializeWeaponData();
        List_Armour.allArmourData = new List<List_Armour>();
        List_Armour.InitializeArmourData();
        List_Consumable.allConsumableData = new List<List_Consumable>();
        List_Consumable.InitializeConsumableData();

        foreach (List_Weapon weaponData in List_Weapon.allWeaponData)
        {
            itemDictionary.Add(weaponData.itemID, weaponData);
        }

        foreach (List_Armour armourData in  List_Armour.allArmourData)
        {
            itemDictionary.Add(armourData.itemID, armourData);
        }

        foreach (List_Consumable consumableData in List_Consumable.allConsumableData)
        {
            itemDictionary.Add(consumableData.itemID, consumableData);
        }

        Debug.Log("itemDictionary count: " + itemDictionary.Count);

        foreach (var item in itemDictionary)
        {
            Debug.Log("itemDictionary item key: " + item.Key + " value: " + item.Value);
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

    public static Manager_Item GetItemData(int itemID)
    {
        if (itemDictionary == null)
        {
            Debug.LogError("itemDictionary is null");
            return null;
        }
        else if (itemDictionary.Count == 0)
        {
            Debug.LogWarning("itemDictionary is empty");
        }
        else
        {
            Debug.Log("itemDictionary has items");
        }

        if (itemDictionary.ContainsKey(itemID))
        {
            Debug.Log(itemID);
            Manager_Item itemData = itemDictionary[itemID];

            if (itemData is Manager_Item)
            {
                Manager_Item managerItemData = itemData;
                Debug.Log("Item data: " + managerItemData);
                return managerItemData;
            }
            else
            {
                Debug.LogWarning($"Item data with ID {itemID} is not of type Manager_Item");
                return null;
            }
            //object itemData = itemDictionary[itemID];

            //switch (itemData)
            //{
            //    case List_Weapon weaponData:
            //        return weaponData;
            //    case List_Armour armourData:
            //        return armourData;
            //    case List_Consumable consumableData:
            //        return consumableData;
            //    default:
            //        Debug.Log("Invalid item type");
            //        break;
            //}
        }

        else
        {
            Debug.LogError("Item ID " + itemID + " does not exist.");
        }

        return null;
    }

    public string ItemToString(int itemID)
    {
        if (itemDictionary == null)
        {
            return null;
        }

        if (itemDictionary.ContainsKey(itemID))
        {
            object itemData = itemDictionary[itemID];

            switch (itemData)
            {
                case List_Weapon weaponData:
                    return "ItemID: " + itemID + ", Type:" + itemType + ", weaponType:" + weaponData.weaponType + ", Name: " + itemName + ", Damage: " + itemDamage + ", Speed: " + itemSpeed + ", Force: " + itemForce + ", Range: " + itemRange + ", Icon: " + itemIcon; ;
                case List_Armour armourData:
                    return "Armour data";
                case List_Consumable consumableData:
                    return "Consumable data";
                default:
                    Debug.Log("Invalid item type");
                    break;
            }
        }

        return null;
    }
}
