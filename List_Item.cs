using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armour,
    Consumable
}
public abstract class List_Item
{
    private static HashSet<int> usedIDs = new HashSet<int>();

    public int itemID;
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize;
    public int itemValue;
    public Vector3 itemScale;
    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public AnimatorController itemAnimatorController;

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

    public static List_Item GetItemData(int itemID, List<List_Item> list)
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
}