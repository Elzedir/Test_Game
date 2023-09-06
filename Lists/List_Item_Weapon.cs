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

public class List_Item_Weapon : List_Item
{
    public static List<List_Item> AllWeaponData = new();

    public List_Item_Weapon
        (int itemID,
        ItemType itemType,
        WeaponType weaponType,
        WeaponClass weaponClass,
        string itemName,
        Sprite itemIcon,
        Vector3 itemPosition,
        Vector3 itemRotation,
        Vector3 itemScale,
        bool equippable,
        AnimatorController itemAnimatorController,
        int maxStackSize,
        int itemValue,
        float itemDamage,
        float itemSpeed,
        float itemForce,
        float itemRange)
    {
        this.ItemStats.CommonStats.ItemID = itemID;
        this.ItemStats.CommonStats.ItemType = itemType;
        this.ItemStats.WeaponStats.WeaponType = weaponType;
        this.ItemStats.WeaponStats.WeaponClass = weaponClass;
        this.ItemStats.CommonStats.ItemName = itemName;
        this.ItemStats.CommonStats.ItemIcon = itemIcon;
        this.ItemStats.CommonStats.ItemPosition = itemPosition;
        this.ItemStats.CommonStats.ItemRotation = itemRotation;
        this.ItemStats.CommonStats.ItemScale = itemScale;
        this.ItemStats.CommonStats.Equippable = equippable;
        this.ItemStats.CommonStats.ItemAnimatorController = itemAnimatorController;
        this.ItemStats.CommonStats.MaxStackSize = maxStackSize;
        this.ItemStats.CommonStats.ItemValue = itemValue;
        this.ItemStats.WeaponStats.ItemDamage = itemDamage;
        this.ItemStats.WeaponStats.ItemSpeed = itemSpeed;
        this.ItemStats.WeaponStats.ItemForce = itemForce;
        this.ItemStats.WeaponStats.ItemRange = itemRange;
    }

    public static void InitializeWeaponData()
    {
        Ranged();
        Shortswords(); //Melee
        
        foreach (var weapon in AllWeaponData)
        {
            weapon.Start();
        }
    }
    static void Ranged()
    {
        Shortbows();
    }
    static void Shortbows()
    {
        List_Item_Weapon testBow1 = new List_Item_Weapon(
            100,
            ItemType.Weapon,
            WeaponType.TwoHandedRanged,
            WeaponClass.Shortbow,
            "Wooden Shortbow",
            SO_List.Instance.WeaponRangedSprites[0].sprite,
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            true,
            SO_List.Instance.animatorControllers[2].animatorController,
            1,
            15,
            1,
            1,
            1,
            3);

        AddToList(AllWeaponData, testBow1);
    }
    static void Shortswords()
    {        
        List_Item_Weapon woodShortsword1 = new List_Item_Weapon(
            1,
            ItemType.Weapon,
            WeaponType.OneHandedMelee,
            WeaponClass.ShortSword,
            "Wood Shortsword",
            SO_List.Instance.WeaponMeleeSprites[0].sprite,
            new Vector3(-0.04f, -0.07f, 0f),
            new Vector3(180, 0, 0),
            new Vector3(0.4f, 0.4f, 0.4f),
            true,
            SO_List.Instance.animatorControllers[0].animatorController,
            3,
            10,
            //GameManager.Randomise(1, 2),
            2,
            2,
            2,
            2);

        AddToList(AllWeaponData, woodShortsword1);
    }

    

    public override string ToString()
    {
        foreach (var weaponData in AllWeaponData)
        {
            if (weaponData.ItemStats.CommonStats.ItemID == ItemStats.CommonStats.ItemID)
            {
                return "ItemID: " + ItemStats.CommonStats.ItemID + 
                    ", Type: " + ItemStats.WeaponStats.WeaponType + 
                    ", Name: " + ItemStats.CommonStats.ItemName +
                    ", Icon: " + (ItemStats.CommonStats.ItemIcon != null ? ItemStats.CommonStats.ItemIcon.name : "null") +
                    ", MaxStackSize: " + ItemStats.CommonStats.MaxStackSize +
                    ", Value: " + ItemStats.CommonStats.ItemValue +
                    ", Damage: " + ItemStats.WeaponStats.ItemDamage + 
                    ", Speed: " + ItemStats.WeaponStats.ItemSpeed + 
                    ", Force: " + ItemStats.WeaponStats.ItemForce + 
                    ", Range: " + ItemStats.WeaponStats.ItemRange;
            }
        }

        return null;
    }
}
