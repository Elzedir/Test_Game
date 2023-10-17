using System;
using System.Collections.Generic;
using UnityEngine;

public enum Ability
{
    None,
    Charge,
    ChargedShot,
    Invulnerability
}

public class List_Ability
{
    public static List<List_Ability> AllAbilityData = new();
    private static HashSet<Ability> _usedAbilities = new();

    public AbilityData AbilityData;

    public static void AddToList(List<List_Ability> list, List_Ability ability)
    {
        if (_usedAbilities.Contains(ability.AbilityData.AbilityStats.AbilityName))
        {
            throw new ArgumentException("Item ID " + ability.AbilityData.AbilityStats.AbilityName + " is already used");
        }

        _usedAbilities.Add(ability.AbilityData.AbilityStats.AbilityName);
        list.Add(ability);
    }

    public static void InitialiseAbilities()
    {
        ArcheryAbilities();
    }

    static void ArcheryAbilities()
    {
        AbilityStats abilityStats = new AbilityStats(
            abilityName: Ability.ChargedShot,
            abilityDescription: "Charge up and fire an arrow of energy",
            abilityIcon: List_Item.GetSpriteFromSpriteSheet(path: "Resources_Sprite/Weapon/Weapons/obj_wep_m_02 (st)", name: "Charged_Shot"),
            abilitySpecialisation: Aspect.Hunt,
            abilityMaxLevel: 5,
            abilityDamagePerAbilityLevel: 2
            );

        CombatStats combatStats = new CombatStats(
            attackDamage: 5,
            attackSpeed: 1,
            attackSwingTime: 4,
            attackRange: 2,
            attackPushForce: 1,
            attackCooldown: 10f
            );

        WeaponStats weaponStats = new WeaponStats(
            weaponType: new WeaponType[] { WeaponType.OneHandedRanged, WeaponType.TwoHandedRanged },
            weaponClass: new WeaponClass[] { WeaponClass.None },
            maxChargeTime: 2
            );

        AbilityData abilityData = new AbilityData(abilityStats: abilityStats, combatStats: combatStats, weaponStats: weaponStats);

        List_Ability chargedShot = new List_Ability_ChargedShot(abilityData: abilityData);

        AddToList(AllAbilityData, chargedShot);
    }

    public static void UnlockAbility(List<List_Ability> unlockedAbilityList, Ability abilityEnum)
    {
        foreach (List_Ability ability in AllAbilityData)
        {
            if (ability.AbilityData.AbilityStats.AbilityName == abilityEnum)
            {
                unlockedAbilityList.Add(ability);
            }
        }
    }

    public virtual void UseAbility(Actor_Base actor)
    {

    }

    //public virtual void Charge(GameObject target, Rigidbody2D self)
    //{
    //    if (target != null)
    //    {
    //        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
    //        bool withinChargeDistance = distanceToTarget < maxCharge && distanceToTarget > minCharge;
    //        bool chargeAvailable = Cooldowns["Charge"] <= 0;

    //        if (withinChargeDistance && chargeAvailable)
    //        {
    //            Cooldowns["Charge"] = chargeCooldown;
    //            Vector2 chargeDirection = target.transform.position - transform.position;
    //            float chargeForce = distanceToTarget;
    //            Vector2 charge = chargeDirection * chargeForce; // * chargeSpeedMultiplier
    //            self.AddForce(charge, ForceMode2D.Impulse);
    //            Debug.Log(this.name + "Charged");
    //        }

    //        if (distanceToTarget > maxCharge)
    //        {
    //            self.velocity = Vector2.zero;
    //        }

    //        else if (distanceToTarget < minCharge)
    //        {
    //            self.velocity = Vector2.zero;
    //        }
    //    }
    //}

    //public virtual void Invulnerability()
    //{
    //    if (!dead)
    //    {
    //        if (Time.time - lastImmune > immuneTime)
    //        {
    //            lastImmune = Time.time;
    //            hitpoint -= dmg.damageAmount;
    //            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
    //            GameManager.instance.ShowFloatingText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.up * 30, 0.5f);

    //            if (hitpoint <= 0)
    //            {
    //                hitpoint = 0;
    //                Death();
    //            }
    //        }
    //    }
    //}

    public static List_Ability GetAbility(Ability abilityName)
    {
        foreach (List_Ability ability in AllAbilityData)
        {
            if (ability.AbilityData.AbilityStats.AbilityName == abilityName)
            {
                return ability;
            }
        }
        return null;
    }
}

[Serializable]
public class AbilityData
{
    public AbilityStats AbilityStats;
    public WeaponStats WeaponStats;
    public CombatStats CombatStats;

    public AbilityData(
        AbilityStats abilityStats = null,
        WeaponStats weaponStats = null,
        CombatStats combatStats = null
        )
    {
        this.AbilityStats = abilityStats != null ? abilityStats : new AbilityStats();
        this.WeaponStats = weaponStats != null ? weaponStats : new WeaponStats();
        this.CombatStats = combatStats != null ? combatStats : new CombatStats();
    }
}

[Serializable]
public class AbilityStats
{
    public Ability AbilityName;
    public string AbilityDescription;
    public Sprite AbilityIcon;
    public Aspect AbilitySpecialisation;
    public int AbilityMaxLevel;
    public float AbilityDamagePerAbilityLevel;
    public ItemType RequiredType;

    public AbilityStats(
        Ability abilityName = Ability.None,
        string abilityDescription = "",
        Sprite abilityIcon = null,
        Aspect abilitySpecialisation = Aspect.None,
        int abilityMaxLevel = 0,
        float abilityDamagePerAbilityLevel = 0,
        ItemType requiredType = ItemType.None
        )
    {
        this.AbilityName = abilityName;
        this.AbilityDescription = abilityDescription;
        this.AbilityIcon = abilityIcon;
        this.AbilitySpecialisation = abilitySpecialisation;
        this.AbilityMaxLevel = abilityMaxLevel;
        this.AbilityDamagePerAbilityLevel = abilityDamagePerAbilityLevel;
        this.RequiredType = requiredType;
    }
}