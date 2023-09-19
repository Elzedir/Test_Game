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

    public Ability AbilityName;
    //public string AbilityDescription;
    public Sprite AbilityIcon;
    public Specialisation AbilitySpecialisation;
    public int AbilityMaxLevel;
    public float AbilityBaseDamage;
    public float AbilityDamagePerAbilityLevel;
    public float AbilityRange;
    public float AbilitySpeed;
    public float AbilityChargeTime;
    public float AbilityCooldown;
    // public Requiredweapon

    public void Start()
    {
        
    }

    public static void AddToList(List<List_Ability> list, List_Ability ability)
    {
        if (_usedAbilities.Contains(ability.AbilityName))
        {
            throw new ArgumentException("Item ID " + ability.AbilityName + " is already used");
        }

        _usedAbilities.Add(ability.AbilityName);
        list.Add(ability);
    }

    public static void InitialiseAbilities()
    {
        ArcheryAbilities();
    }

    static void ArcheryAbilities()
    {
        List_Ability chargedShot = new List_Ability_ChargedShot(
            Ability.ChargedShot,
            Specialisation.Archery,
            5,
            5,
            2,
            3,
            1.5f,
            3f,
            5f,
            SO_List.Instance.WeaponMeleeSprites[0].sprite);
        AddToList(AllAbilityData, chargedShot);
    }

    public static void UnlockAbility(List<List_Ability> unlockedAbilityList, Ability abilityEnum)
    {
        foreach (List_Ability ability in AllAbilityData)
        {
            if (ability.AbilityName == abilityEnum)
            {
                unlockedAbilityList.Add(ability);
            }
        }
    }

    public virtual void UseAbility(Actor_Base actor)
    {
        throw new ArgumentException("Ability " + this.AbilityName + " does not have a UseAbility");
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
            if (ability.AbilityName == abilityName)
            {
                return ability;
            }
        }
        return null;
    }
}