using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

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
        string weaponName,
        int maxStackSize,
        int weaponValue,
        float weaponDamage,
        float weaponSpeed,
        float weaponForce,
        float weaponRange,
        Sprite weaponIcon)
    {
        this.itemID = itemID;
        this.weaponType = weaponType;
        this.itemName = weaponName;
        this.maxStackSize = maxStackSize;
        this.itemValue = weaponValue;
        this.weaponDamage = weaponDamage;
        this.weaponSpeed = weaponSpeed;
        this.weaponForce = weaponForce;
        this.weaponRange = weaponRange;
        this.itemIcon = weaponIcon;
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
        Sprite shortsword_Wood_spr = Resources.Load<Sprite>("Assets/Artwork/Assets/0_Assets/Atlas/at_dungeon_01/obj_wep_m_ss_01");
        List_Weapon shortSwordData = new List_Weapon(
            1,
            WeaponType.Shortsword,
            "Wood shortsword",
            3,
            10,
            GameManager.Randomise(1, 2),
            GameManager.Randomise(1, 1.3),
            GameManager.Randomise(1, 1.3),
            GameManager.Randomise(1, 1.3),
            shortsword_Wood_spr);

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
                    ", Damage: " + weaponDamage + 
                    ", Speed: " + weaponSpeed + 
                    ", Force: " + weaponForce + 
                    ", Range: " + weaponRange + 
                    ", Icon: " + itemIcon;
            }
        }

        return null;
    }
}
