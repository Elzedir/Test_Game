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
        string itemName,
        Sprite itemIcon,
        Vector3 itemPosition,
        Vector3 itemRotation,
        Vector3 itemScale,
        bool equippable,
        AnimatorController itemAnimatorController,
        int maxStackSize,
        int itemValue,
        float itemHealth,
        float itemMana,
        float itemStamina,
        float itemPushRecovery,
        float itemAttackDamage,
        float itemAttackSpeed,
        float itemAttackSwingTime,
        float itemAttackRange,
        float itemAttackPushForce,
        float itemAttackCooldown,
        float itemPhysicalDefence,
        float itemMagicalDefence,
        float itemDodgeCooldown,
        WeaponType weaponType,
        WeaponClass weaponClass,
        float maxChargeTime)
    {
        this.ItemStats.CommonStats.ItemID = itemID;
        this.ItemStats.CommonStats.ItemType = itemType;
        this.ItemStats.CommonStats.ItemName = itemName;
        this.ItemStats.CommonStats.ItemIcon = itemIcon;
        this.ItemStats.CommonStats.ItemPosition = itemPosition;
        this.ItemStats.CommonStats.ItemRotation = itemRotation;
        this.ItemStats.CommonStats.ItemScale = itemScale;
        this.ItemStats.CommonStats.Equippable = equippable;
        this.ItemStats.CommonStats.ItemAnimatorController = itemAnimatorController;
        this.ItemStats.CommonStats.MaxStackSize = maxStackSize;
        this.ItemStats.CommonStats.ItemValue = itemValue;
        this.ItemStats.CombatStats.Health = itemHealth;
        this.ItemStats.CombatStats.Mana = itemMana;
        this.ItemStats.CombatStats.Stamina = itemStamina;
        this.ItemStats.CombatStats.PushRecovery = itemPushRecovery;
        this.ItemStats.CombatStats.AttackDamage = itemAttackDamage;
        this.ItemStats.CombatStats.AttackSpeed = itemAttackSpeed;
        this.ItemStats.CombatStats.AttackSwingTime = itemAttackSwingTime;
        this.ItemStats.CombatStats.AttackRange = itemAttackRange;
        this.ItemStats.CombatStats.AttackPushForce = itemAttackPushForce;
        this.ItemStats.CombatStats.AttackCooldown = itemAttackCooldown;
        this.ItemStats.CombatStats.PhysicalDefence = itemPhysicalDefence;
        this.ItemStats.CombatStats.MagicalDefence = itemMagicalDefence;
        this.ItemStats.CombatStats.DodgeCooldown = itemDodgeCooldown;
        this.ItemStats.WeaponStats.WeaponType = weaponType;
        this.ItemStats.WeaponStats.WeaponClass = weaponClass;
        this.ItemStats.WeaponStats.MaxChargeTime = maxChargeTime;
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
            100, // ItemID
            ItemType.Weapon, // ItemType
            "Wooden Shortbow", // ItemName
            SO_List.Instance.WeaponRangedSprites[0].sprite, // ItemIcon
            new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), // ItemPosition, ItemRotation, ItemScale
            true, // ItemEquippable
            SO_List.Instance.animatorControllers[2].animatorController, // ItemAnimatorController
            1, // ItemMaxStackSize
            15, // ItemValue
            0, // ItemHealth
            0, // ItemMana
            0, // ItemStamina
            0, // ItemPushRecovery
            1, // ItemAttackDamage
            1, // itemAttackSpeed,
            1, // itemAttackSwingTime,
            3, // itemAttackRange,
            1.5f, // itemAttackPushForce,
            1, // itemAttackCooldown,
            0, // itemPhysicalDefence,
            0, // itemMagicalDefence,
            0, // itemDodgeCooldown,
            WeaponType.TwoHandedRanged, // WeaponType
            WeaponClass.Shortbow, // WeaponClass
            3); // ItemMaxChargeTime

        AddToList(AllWeaponData, testBow1);
    }
    static void Shortswords()
    {        
        List_Item_Weapon woodShortsword1 = new List_Item_Weapon(
            1, // ItemID
            ItemType.Weapon, // ItemType
            "Wood Shortsword", // ItemName
            SO_List.Instance.WeaponMeleeSprites[0].sprite, // ItemIcon
            new Vector3(-0.04f, -0.07f, 0f), new Vector3(180, 0, 0), new Vector3(0.4f, 0.4f, 0.4f),// ItemPosition, ItemRotation, ItemScale
            true,// ItemEquippable
            SO_List.Instance.animatorControllers[0].animatorController,// ItemAnimatorController
            1, // ItemMaxStackSize
            15, // ItemValue
            0, // ItemHealth
            0, // ItemMana
            0, // ItemStamina
            0, // ItemPushRecovery
            1, // ItemAttackDamage
            2, // itemAttackSpeed,
            1, // itemAttackSwingTime,
            1, // itemAttackRange,
            2f, // itemAttackPushForce,
            1, // itemAttackCooldown,
            0, // itemPhysicalDefence,
            0, // itemMagicalDefence,
            0, // itemDodgeCooldown,
            WeaponType.OneHandedMelee, // WeaponType
            WeaponClass.ShortSword, // WeaponClass
            3); // ItemMaxChargeTime);

        AddToList(AllWeaponData, woodShortsword1);
    }
}
