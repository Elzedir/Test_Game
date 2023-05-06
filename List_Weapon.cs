using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum WeaponType
{
    Shortsword,
    Axe,
    Spear
}

public class List_Weapon : List_Item
{
    public static List<List_Item> allWeaponData = new List<List_Item>();

    public WeaponType weaponType;
    public float weaponDamage;
    public float weaponSpeed;
    public float weaponForce;
    public float weaponRange;

    public List_Weapon
        (int itemID,
        WeaponType weaponType,
        string itemName,
        Sprite itemIcon,
        int maxStackSize,
        int weaponValue,
        float weaponDamage,
        float weaponSpeed,
        float weaponForce,
        float weaponRange)
    {
        this.itemID = itemID;
        this.weaponType = weaponType;
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.maxStackSize = maxStackSize;
        this.itemValue = weaponValue;
        this.weaponDamage = weaponDamage;
        this.weaponSpeed = weaponSpeed;
        this.weaponForce = weaponForce;
        this.weaponRange = weaponRange;
    }

    public static void InitializeWeaponData()
    {
        Shortswords();

        foreach (var weapon in allWeaponData)
        {
            weapon.Start();
        }

        //foreach (var item in allWeaponData)
        //{
        //    Debug.Log("item itemID: " + item.itemID + ", itemName: " + item.itemName);
        //}
    }

    static void Shortswords()
    {
        Sprite[] at_dungeon_01 = Resources.LoadAll<Sprite>("at_dungeon_01");
        Sprite weaponSprite = null;

        foreach (Sprite sprite in at_dungeon_01)
        {
            if (sprite.name == "obj_wep_m_ss_01")
            {
                weaponSprite = sprite;
            }
        }

        Debug.Log(weaponSprite);

        List_Weapon shortSwordData = new List_Weapon(
            1,
            WeaponType.Shortsword,
            "Wood shortsword",
            weaponSprite,
            3,
            10,
            GameManager.Randomise(1, 2),
            GameManager.Randomise(1, 1.3),
            GameManager.Randomise(1, 1.3),
            GameManager.Randomise(1, 1.3));

        AddToList(allWeaponData, shortSwordData);
    }

    public override string ToString()
    {
        foreach (var weaponData in allWeaponData)
        {
            if (weaponData.itemID == itemID)
            {
                return "ItemID: " + itemID + 
                    ", Type: " + weaponType + 
                    ", Name: " + itemName +
                    ", Icon: " + (itemIcon != null ? itemIcon.name : "null") +
                    ", MaxStackSize: " + maxStackSize +
                    ", Value: " + itemValue +
                    ", Damage: " + weaponDamage + 
                    ", Speed: " + weaponSpeed + 
                    ", Force: " + weaponForce + 
                    ", Range: " + weaponRange;
            }
        }

        return null;
    }
}
