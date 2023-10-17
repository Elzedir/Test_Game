using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemType
{
    None,
    Weapon,
    Armour,
    Consumable,
    Unknown
}

public abstract class List_Item
{
    private static HashSet<int> _usedIDs = new HashSet<int>();
    public ItemStats ItemStats;

    public virtual void Start()
    {
       
    }

    public static void AddToList(List<List_Item> list, List_Item item)
    {
        if (_usedIDs.Contains(item.ItemStats.CommonStats.ItemID))
        {
            throw new ArgumentException("Item ID " + item.ItemStats.CommonStats.ItemID + " is already used");
        }

        _usedIDs.Add(item.ItemStats.CommonStats.ItemID);
        list.Add(item);
    }

    public static List_Item GetItemData(int itemID)
    {
        List<List_Item>[] allLists = new List<List_Item>[]
        {
        List_Item_Weapon.AllWeaponData,
        List_Item_Armour.AllArmourData,
        List_Item_Consumable.allConsumableData
        };

        foreach (var list in allLists)
        {
            List_Item foundItem = SearchItemInList(itemID, list);
            if (foundItem != null)
            {
                return foundItem;
            }
        }
        return null;
    }

    private static List_Item SearchItemInList(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.ItemStats.CommonStats.ItemID == itemID)
            {
                return data;
            }
        }

        return null;
    }

    public static Sprite GetItemSprite(int itemID, List<List_Item> list)
    {
        foreach (var data in list)
        {
            if (data.ItemStats.CommonStats.ItemID == itemID)
            {
                return data.ItemStats.CommonStats.ItemIcon;
            }
        }

        return null;
    }

    public void AttachWeaponScript(List_Item item, Equipment_Slot equipmentSlot)
    {
        GameManager.Destroy(equipmentSlot.GetComponent<Weapon>());

        foreach (WeaponType weaponType in item.ItemStats.WeaponStats.WeaponTypeArray)
        {
            switch (weaponType)
            {
                case WeaponType.OneHandedMelee:
                case WeaponType.TwoHandedMelee:
                    foreach (WeaponClass weaponClass in item.ItemStats.WeaponStats.WeaponClassArray)
                    {
                        switch (weaponClass)
                        {
                            case WeaponClass.Axe:
                                //equipmentSlot.AddComponent<Weapon_Axe>();
                                break;
                            case WeaponClass.ShortSword:
                                //equipmentSlot.AddComponent<Weapon_ShortSword>();
                                break;
                                // Add more cases here
                        }
                    }
                    break;
                case WeaponType.OneHandedRanged:
                case WeaponType.TwoHandedRanged:
                    equipmentSlot.AddComponent<Weapon_Bow>();
                    break;
                case WeaponType.OneHandedMagic:
                case WeaponType.TwoHandedMagic:
                    foreach (WeaponClass weaponClass in item.ItemStats.WeaponStats.WeaponClassArray)
                    {
                        //switch (weaponClass)
                        //{
                        //    case WeaponClass.Staff:
                        //        equipmentSlot.AddComponent<Weapon_Staff>();
                        //        break;
                        //    case WeaponClass.Wand:
                        //        equipmentSlot.AddComponent<Weapon_Wand>();
                        //        break;
                        //         Add more cases here
                        //}
                    }
                    break;
            }
        }
    }

    public static Sprite GetSpriteFromSpriteSheet(string path, string name = null, int index = -1)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        Sprite sprite = null;

        if (name != null)
        {
            sprite = System.Array.Find(sprites, s => s.name.Equals(name));
        }
        else if (index != -1)
        {
            Sprite specificSpriteByIndex = sprites[index];
        }

        return sprite;
    }
}

[Serializable]
public class ItemStats
{
    public CommonStats CommonStats;
    public WeaponStats WeaponStats;
    public ArmourStats ArmourStats;
    public FixedModifiers StatModifiersFixed;
    public PercentageModifiers StatModifiersPercentage;

    public ItemStats(
        CommonStats commonStats = null,
        WeaponStats weaponStats = null, 
        ArmourStats armourStats = null, 
        FixedModifiers fixedModifiers = null, 
        PercentageModifiers percentageModifiers = null
        )
    {
        this.CommonStats = commonStats != null ? commonStats : new CommonStats();
        this.WeaponStats = weaponStats != null ? weaponStats : new WeaponStats();
        this.ArmourStats = armourStats != null ? armourStats : new ArmourStats();
        this.StatModifiersFixed = fixedModifiers != null ? fixedModifiers : new FixedModifiers();
        this.StatModifiersPercentage = percentageModifiers != null ? percentageModifiers : new PercentageModifiers();
    }

    public static ItemStats GetItemStats(int itemID, int stackSize)
    {
        if (itemID != -1 && stackSize != 0)
        {
            ItemStats newItemStats = List_Item.GetItemData(itemID).ItemStats;
            newItemStats.CommonStats.CurrentStackSize = stackSize;
            return newItemStats;
        }
        else
        {
            return new ItemStats();
        }
    }
}

[Serializable]
public class CommonStats
{
    public int ItemID;
    public ItemType ItemType;
    public string ItemName;
    public Sprite ItemIcon;
    public int MaxStackSize;
    public int CurrentStackSize;
    public int ItemValue;
    public float ItemWeight;
    public bool ItemEquippable;
    public Vector3 ItemPosition;
    public Vector3 ItemRotation;
    public Vector3 ItemScale;
    public RuntimeAnimatorController ItemAnimatorController;

    public CommonStats(
        int itemID = -1,
        ItemType itemType = ItemType.None,
        string itemName = "",
        Sprite itemIcon = null,
        int maxStackSize = 0,
        int currentStackSize = 0,
        int itemValue = 0,
        float itemWeight = 0,
        bool itemEquippable = false,
        Vector3? itemPosition = null,
        Vector3? itemRotation = null,
        Vector3? itemScale = null,
        RuntimeAnimatorController itemAnimatorController = null
        )
    {
        this.ItemID = itemID;
        this.ItemType = itemType;
        this.ItemName = itemName;
        this.ItemIcon = itemIcon;
        this.MaxStackSize = maxStackSize;
        this.CurrentStackSize = currentStackSize;
        this.ItemValue = itemValue;
        this.ItemWeight = itemWeight;
        this.ItemEquippable = itemEquippable;
        this.ItemPosition = itemPosition ?? Vector3.zero;
        this.ItemRotation = itemRotation ?? Vector3.zero;
        this.ItemScale = itemScale ?? new Vector3(1, 1, 1);
        this.ItemAnimatorController = itemAnimatorController;
    }
}

[Serializable]
public class PercentageModifiers
{
    public float CurrentHealth;
    public float CurrentMana;
    public float CurrentStamina;
    public float MaxHealth;
    public float MaxMana;
    public float MaxStamina;
    public float PushRecovery;

    public float AttackDamage;
    public float AttackSpeed;
    public float AttackSwingTime;
    public float AttackRange;
    public float AttackPushForce;
    public float AttackCooldown;

    public float PhysicalDefence;
    public float MagicalDefence;

    public float MoveSpeed;
    public float DodgeCooldownReduction;

    public PercentageModifiers(
        float maxHealth = 1,
        float maxMana = 1,
        float maxStamina = 1,
        float pushRecovery = 1,

        float attackDamage = 1,
        float attackSpeed = 1,
        float attackSwingTime = 1,
        float attackRange = 1,
        float attackPushForce = 1,
        float attackCooldown = 1,

        float physicalDefence = 1,
        float magicalDefence = 1,

        float moveSpeed = 1,
        float dodgeCooldownReduction = 1
        )
    {
        this.MaxHealth = maxHealth;
        this.MaxMana = maxMana;
        this.MaxStamina = maxStamina;
        this.PushRecovery = pushRecovery;
        this.AttackDamage = attackDamage;
        this.AttackSpeed = attackSpeed;
        this.AttackSwingTime = attackSwingTime;
        this.AttackRange = attackRange;
        this.AttackPushForce = attackPushForce;
        this.AttackCooldown = attackCooldown;
        this.PhysicalDefence = physicalDefence;
        this.MagicalDefence = magicalDefence;
        this.MoveSpeed = moveSpeed;
        this.DodgeCooldownReduction = dodgeCooldownReduction;
    }
}

[Serializable]
public class FixedModifiers
{
    public float CurrentHealth;
    public float CurrentMana;
    public float CurrentStamina;
    public float MaxHealth;
    public float MaxMana;
    public float MaxStamina;
    public float PushRecovery;

    public float AttackDamage;
    public float AttackSpeed;
    public float AttackSwingTime;
    public float AttackRange;
    public float AttackPushForce;
    public float AttackCooldown;

    public float PhysicalDefence;
    public float MagicalDefence;

    public float MoveSpeed;
    public float DodgeCooldownReduction;

    // public float DamagePerAbilityLevel;
    // public float DamagePerItemLevel;
    // public SPECIAL SPECIAL;

    public FixedModifiers(
        float maxHealth = 0,
        float maxMana = 0,
        float maxStamina = 0,
        float pushRecovery = 0,

        float attackDamage = 0,
        float attackSpeed = 0,
        float attackSwingTime = 0,
        float attackRange = 0,
        float attackPushForce = 0,
        float attackCooldown = 0,

        float physicalDefence = 0,
        float magicalDefence = 0,

        float moveSpeed = 0,
        float dodgeCooldownReduction = 0
        )
    {
        this.MaxHealth = maxHealth;
        this.MaxMana = maxMana;
        this.MaxStamina = maxStamina;
        this.PushRecovery = pushRecovery;
        this.AttackDamage = attackDamage;
        this.AttackSpeed = attackSpeed;
        this.AttackSwingTime = attackSwingTime;
        this.AttackRange = attackRange;
        this.AttackPushForce = attackPushForce;
        this.AttackCooldown = attackCooldown;
        this.PhysicalDefence = physicalDefence;
        this.MagicalDefence = magicalDefence;
        this.MoveSpeed = moveSpeed;
        this.DodgeCooldownReduction = dodgeCooldownReduction;
    }
}

[Serializable]
public class WeaponStats
{
    public WeaponType[] WeaponTypeArray;
    public WeaponClass[] WeaponClassArray;
    public float MaxChargeTime;

    public WeaponStats(
        WeaponType[] weaponType = null, 
        WeaponClass[] weaponClass = null, 
        float maxChargeTime = 0
        )
    {
        if (weaponType == null)
        {
            weaponType = new WeaponType[] { WeaponType.None };
        }
        if (weaponClass == null)
        {
            weaponClass = new WeaponClass[] { WeaponClass.None };
        }

        this.WeaponTypeArray = weaponType;
        this.WeaponClassArray = weaponClass;
        this.MaxChargeTime = maxChargeTime;
    }
}