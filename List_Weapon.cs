using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class List_Weapon : List_Item
{
    public static List<List_Weapon> allWeaponData;

    public List_Weapon(int itemID, string itemName, string itemType, float itemDamage, float itemSpeed, float itemForce, float itemRange, Sprite itemIcon) : base(itemID, itemName)
    {
        List_Item.itemID = itemID;
        this.itemName = itemName;
        this.itemType = itemType;
        this.itemDamage = itemDamage;
        this.itemSpeed = itemSpeed;
        this.itemForce = itemForce;
        this.itemRange = itemRange;
        this.itemIcon = itemIcon;
    }

    public static void InitializeWeaponData()
    {
        Shortswords();
    }

    static void Shortswords()
    {
        Sprite shortsword_Wood_spr = Resources.Load<Sprite>("Assets/Artwork/Assets/0_Assets/Atlas/at_dungeon_01/obj_wep_m_ss_01");
        List_Weapon shortSwordData = new List_Weapon(0, "Bronze shortsword", GameManager.Randomise(1, 2), GameManager.Randomise(1, 1.3), GameManager.Randomise(1, 1.3), GameManager.Randomise(1, 1.3), shortsword_Wood_spr);
        allWeaponData.Add(shortSwordData);
    }

    public string WepToString()
    {
        return "ItemID: " + itemID + ", Name: " + itemName + "Type" + itemType + ", Damage: " + itemDamage + ", Speed: " + itemSpeed + ", Force: " + itemForce + ", Range: " + itemRange + ", Icon" + wepIcon;
    }
}


