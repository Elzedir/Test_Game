using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
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

    public void Start()
    {
        
    }

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
        List_Ability chargedShot = new List_Ability_ChargedShot(
            Ability.ChargedShot, // Name
            "Charge up and fire an arrow of energy", // Description
            SO_List.Instance.WeaponMeleeSprites[0].sprite, // Icon
            Aspect.Hunt, // Specialisation
            5, // Max Ability Level
            2, // Damage Per Level
            ItemType.Weapon, // Required Type
            0, // Health
            0, // Mana
            0, // Stamina
            0, // Push Recovery
            5, // Ability Damage
            1, // Ability Speed (Bow draw speed)
            4, // Ability Swing Time (Arrow speed)
            4, // Ability Range
            1, // Ability Push Force
            10f, // AbilityCooldown
            0, 0, // Physical, Magical Defence
            0, // DodgeCooldown

            new WeaponType[] { WeaponType.OneHandedRanged, WeaponType.TwoHandedRanged }, // Weapon Type Restrictions
            new WeaponClass[] { WeaponClass.None }, // Weapon Class Restrictions
            2f // ChargeTime
            );
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
public struct AbilityData
{
    public AbilityStats AbilityStats;
    public CombatStats CombatStats;
    public WeaponStats WeaponStats;
    public ArmourStats ArmourStats;
}

[Serializable]
public struct AbilityStats
{
    public Ability AbilityName;
    public string AbilityDescription;
    public Sprite AbilityIcon;
    public Aspect AbilitySpecialisation;
    public int AbilityMaxLevel;
    public float AbilityDamagePerAbilityLevel;
    public ItemType RequiredType;
}