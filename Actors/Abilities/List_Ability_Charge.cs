using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Ability_Charge : List_Ability
{
    public List_Ability_Charge
        (Ability name,
        Aspect specialisation,
        int maxLevel,
        float baseDamage,
        float damagePerAbilityLevel,
        float range,
        float speed,
        float chargeTime,
        Sprite icon)
    {
        this.AbilityData.AbilityStats.AbilityName = name;
        this.AbilityData.AbilityStats.AbilitySpecialisation = specialisation;
        this.AbilityData.AbilityStats.AbilityMaxLevel = maxLevel;
        this.AbilityData.CombatStats.AttackDamage = baseDamage;
        this.AbilityData.AbilityStats.AbilityDamagePerAbilityLevel = damagePerAbilityLevel;
        this.AbilityData.CombatStats.AttackRange = range;
        this.AbilityData.CombatStats.AttackSpeed = speed;
        this.AbilityData.WeaponStats.MaxChargeTime = chargeTime;
        this.AbilityData.AbilityStats.AbilityIcon = icon;
    }

    public override void UseAbility(Actor_Base actor)
    {
        base.UseAbility(actor);
    }
}
