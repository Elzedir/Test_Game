using NUnit.Framework.Constraints;
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

public enum WeaponType
{
    None,
    OneHandedMelee,
    TwoHandedMelee,
    OneHandedRanged,
    TwoHandedRanged,
    OneHandedMagic,
    TwoHandedMagic
}

public enum WeaponClass
{
    None,
    Axe,
    ShortBow,
    ShortSword,
    Spear
}

public class List_Item_Weapon : List_Item
{
    public static List<List_Item> AllWeaponData = new();

    public List_Item_Weapon (ItemStats itemStats) { this.ItemStats = itemStats; }

    public static void InitializeWeaponData()
    {
        Melee();
        Ranged();
        
        foreach (var weapon in AllWeaponData)
        {
            weapon.Start();
        }
    }
    static void Melee()
    {
        ShortSwords();
    }
    static void Ranged()
    {
        ShortBows();
    }
    static void ShortBows()
    {
        CommonStats commonStats = new CommonStats(
            itemID: 100,
            itemType: ItemType.Weapon,
            itemName: "Wooden ShortBow",
            itemIcon: Resources.Load<Sprite>("Resources_Sprite/Weapon/Weapons/bow/bow"),
            itemPosition: Vector3.zero,
            itemRotation: Vector3.zero,
            itemScale: Vector3.one,
            itemEquippable: true,
            itemAnimatorController: Resources.Load<AnimatorController>("Resources_AnimatorControllers/wep_r_sb_01"),
            maxStackSize: 1,
            itemValue: 15
            );

        WeaponStats weaponStats = new WeaponStats(
            weaponType: new WeaponType[] { WeaponType.TwoHandedRanged },
            weaponClass: new WeaponClass[] { WeaponClass.ShortBow },
            maxChargeTime: 2
            );

        FixedModifiers fixedModifiers = new FixedModifiers(
            attackRange: 1
            );

        PercentageModifiers percentageModifiers = new PercentageModifiers(
            attackDamage: 1.1f,
            attackSpeed: 1.5f,
            attackSwingTime: 3f,
            attackRange: 3f,
            attackPushForce: 1.1f
            );

        ItemStats testBow1 = new ItemStats(commonStats: commonStats, weaponStats: weaponStats, fixedModifiers: fixedModifiers, percentageModifiers: percentageModifiers);

        List_Item_Weapon testBow1Weapon = new List_Item_Weapon(testBow1);

        AddToList(AllWeaponData, testBow1Weapon);
    }
    static void ShortSwords()
    {
        CommonStats commonStats = new CommonStats(
            itemID: 1,
            itemType: ItemType.Weapon,
            itemName: "Wooden ShortSword",
            itemIcon: List_Item.GetSpriteFromSpriteSheet(path: "Resources_Sprite/Weapon/Weapons/at_dungeon_01", name: "obj_wep_m_ss_01"),
            itemPosition: new Vector3 (-0.04f, -0.07f, 0),
            itemRotation: new Vector3 (180, 0, 0),
            itemScale: new Vector3(0.4f, 0.4f, 0.4f),
            itemEquippable: true,
            itemAnimatorController: Resources.Load<AnimatorController>("Resources_AnimatorControllers/wep_m_ss_base"),
            maxStackSize: 1,
            itemValue: 15
            );

        WeaponStats weaponStats = new WeaponStats(
            weaponType: new WeaponType[] { WeaponType.OneHandedMelee },
            weaponClass: new WeaponClass[] { WeaponClass.ShortSword },
            maxChargeTime: 3
            );

        FixedModifiers fixedModifiers = new FixedModifiers(
            );

        PercentageModifiers percentageModifiers = new PercentageModifiers(
            attackDamage: 1.2f,
            attackSpeed: 1.1f,
            attackSwingTime: 1.1f,
            attackPushForce: 1.1f
            );

        ItemStats woodenShortSword01 = new ItemStats(commonStats: commonStats, weaponStats: weaponStats, fixedModifiers: fixedModifiers, percentageModifiers: percentageModifiers);

        List_Item_Weapon woodenShortSword01Weapon = new List_Item_Weapon(woodenShortSword01);

        AddToList(AllWeaponData, woodenShortSword01Weapon);
    }
}
