using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class List_Item
{
    private static HashSet<int> usedIDs = new HashSet<int>();

    public int itemID;
    public string itemName;
    public Sprite itemIcon;
    public int maxStackSize;
    public int itemValue;
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
}