using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Ability_ChargedShot : List_Ability
{
    public List_Ability_ChargedShot
        (Ability name,
        Specialisation specialisation,
        int maxLevel,
        float baseDamage,
        float damagePerAbilityLevel,
        float range,
        float speed,
        float chargeTime,
        Sprite icon)
    {
        this.AbilityName = name;
        this.AbilitySpecialisation = specialisation;
        this.AbilityMaxLevel = maxLevel;
        this.AbilityBaseDamage = baseDamage;
        this.AbilityDamagePerAbilityLevel = damagePerAbilityLevel;
        this.AbilityRange = range;
        this.AbilitySpeed = speed;
        this.AbilityChargeTime = chargeTime;
        this.AbilityIcon = icon;
    }

    public override void UseAbility()
    {
        // Play a certain animation.

        // Shoot a specific arrow
        // Play a specific sound
    }
}
