using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Item
{
    private static HashSet<int> usedIDs = new HashSet<int>();

    protected static int itemID;
    protected string itemName;
    protected string itemType;
    protected float itemValue;
    protected Sprite itemIcon;
    protected float itemDamage;
    protected float itemSpeed;
    protected float itemForce;
    protected float itemRange;

    public List_Item (int id, string name)
    {
        if (usedIDs.Contains(id))
        {
            throw new ArgumentException("Item ID " + id + " is already used!");
        }

        itemID = id;
        itemName = name;

        usedIDs.Add(id);
    }
}
