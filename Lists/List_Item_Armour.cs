using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class List_Item_Armour : List_Item
{
    public static List<List_Item> allArmourData = new();

    public static void InitializeArmourData()
    {
        Heavy();

        foreach (var armour in allArmourData)
        {
            armour.Start();
        }
    }

    public List_Item_Armour
        (int itemID,
        ItemType itemType,
        string itemName,
        Sprite itemIcon,
        Vector3 itemPosition,
        Vector3 itemRotation,
        Vector3 itemScale,
        bool equippable,
        // AnimatorController itemAnimatorController,
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
        ArmourType itemArmourType,
        float itemCoverage)
    {
        this.ItemStats.CommonStats.ItemID = itemID;
        this.ItemStats.CommonStats.ItemType = itemType;
        this.ItemStats.CommonStats.ItemName = itemName;
        this.ItemStats.CommonStats.ItemIcon = itemIcon;
        this.ItemStats.CommonStats.ItemPosition = itemPosition;
        this.ItemStats.CommonStats.ItemRotation = itemRotation;
        this.ItemStats.CommonStats.ItemScale = itemScale;
        this.ItemStats.CommonStats.Equippable = equippable;
        // this.ItemStats.CommonStats.ItemAnimatorController = itemAnimatorController;
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
        this.ItemStats.ArmourStats.ArmourType = itemArmourType;
        this.ItemStats.ArmourStats.ItemCoverage = itemCoverage;
    }

    static void Heavy()
    {
        List_Item_Armour bronzeChestplate01 = new List_Item_Armour(
            2, // ItemID
            ItemType.Armour, // ItemType
            "Bronze chestplate", // ItemName
            SO_List.Instance.ArmourSprites[0].sprite, // ItemIcon
            new Vector3(-0.04f, -0.07f, 0f), new Vector3(180, 0, 0), new Vector3(0.4f, 0.4f, 0.4f), // ItemPosition, ItemRotation, ItemScale
            true, // ItemEquippable
            //SO_List.Instance.animatorControllers[0].animatorController, // ItemAnimatorController
            1, // ItemMaxStackSize
            10, // ItemValue
            5, // ItemHealth
            0, // ItemMana
            0, // ItemStamina
            1, // ItemPushRecovery
            0, // ItemAttackDamage
            0, // itemAttackSpeed,
            0, // itemAttackSwingTime,
            0, // itemAttackRange,
            0f, // itemAttackPushForce,
            1, // itemAttackCooldown,
            2, // itemPhysicalDefence,
            2, // itemMagicalDefence,
            0, // itemDodgeCooldown,
            ArmourType.Chest, // ArmourType
            75 // ItemCoverage
            ); 

        AddToList(allArmourData, bronzeChestplate01);
    }
}
