using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class List_Weapon : List_Item
{
    public static List<List_Item> allWeaponData = new List<List_Item>();

    public List_Weapon
        (int itemID,
        ItemType itemType,
        WeaponType weaponType,
        string itemName,
        Sprite itemIcon,
        Vector3 itemScale,
        Vector3 itemPosition,
        Vector3 itemRotation,
        AnimatorController itemAnimatorController,
        int maxStackSize,
        int itemValue,
        float itemDamage,
        float itemSpeed,
        float itemForce,
        float itemRange)
    {
        this.itemID = itemID;
        this.itemType = itemType;
        this.weaponType = weaponType;
        this.itemName = itemName;
        this.itemIcon = itemIcon;
        this.itemScale = itemScale;
        this.itemPosition = itemPosition;
        this.itemRotation = itemRotation;
        this.itemAnimatorController = itemAnimatorController;
        this.maxStackSize = maxStackSize;
        this.itemValue = itemValue;
        this.itemDamage = itemDamage;
        this.itemSpeed = itemSpeed;
        this.itemForce = itemForce;
        this.itemRange = itemRange;
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
        List_Weapon shortSwordData = new List_Weapon(
            1,
            ItemType.Weapon,
            WeaponType.Shortsword,
            "Wood shortsword",
            Game_Settings.Instance.sprites[0].sprite,
            new Vector3(0.4f, 0.4f, 0.4f),
            new Vector3(-0.04f, -0.07f, 0f),
            new Vector3(180, 0, 0),
            Game_Settings.Instance.animatorControllers[1].animatorController,
            3,
            10,
            //GameManager.Randomise(1, 2),
            2,
            2,
            2,
            2);

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
                    ", Damage: " + itemDamage + 
                    ", Speed: " + itemSpeed + 
                    ", Force: " + itemForce + 
                    ", Range: " + itemRange;
            }
        }

        return null;
    }
}
