using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class List_Ability_Charge : List_Ability
{
    public List_Ability_Charge
        (Ability name,
        string description,
        Sprite icon,
        Aspect specialisation,
        int maxLevel,
        float damagePerAbilityLevel,
        ItemType requiredType,
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
        WeaponType[] weaponType,
        WeaponClass[] weaponClass,
        float maxChargeTime)
    {
        this.AbilityData.AbilityStats.AbilityName = name;
        this.AbilityData.AbilityStats.AbilityDescription = description;
        this.AbilityData.AbilityStats.AbilityIcon = icon;
        this.AbilityData.AbilityStats.AbilitySpecialisation = specialisation;
        this.AbilityData.AbilityStats.AbilityMaxLevel = maxLevel;
        this.AbilityData.AbilityStats.AbilityDamagePerAbilityLevel = damagePerAbilityLevel;
        this.AbilityData.AbilityStats.RequiredType = requiredType;

        this.AbilityData.CombatStats.MaxHealth = itemHealth;
        this.AbilityData.CombatStats.MaxMana = itemMana;
        this.AbilityData.CombatStats.MaxStamina = itemStamina;
        this.AbilityData.CombatStats.PushRecovery = itemPushRecovery;
        this.AbilityData.CombatStats.AttackDamage = itemAttackDamage;
        this.AbilityData.CombatStats.AttackSpeed = itemAttackSpeed;
        this.AbilityData.CombatStats.AttackSwingTime = itemAttackSwingTime;
        this.AbilityData.CombatStats.AttackRange = itemAttackRange;
        this.AbilityData.CombatStats.AttackPushForce = itemAttackPushForce;
        this.AbilityData.CombatStats.AttackCooldown = itemAttackCooldown;
        this.AbilityData.CombatStats.PhysicalDefence = itemPhysicalDefence;
        this.AbilityData.CombatStats.MagicalDefence = itemMagicalDefence;
        this.AbilityData.CombatStats.DodgeCooldownReduction = itemDodgeCooldown;

        this.AbilityData.WeaponStats.WeaponTypeArray = weaponType;
        this.AbilityData.WeaponStats.WeaponClassArray = weaponClass;
        this.AbilityData.WeaponStats.MaxChargeTime = maxChargeTime;
    }

    public override void UseAbility(Actor_Base actor)
    {
        base.UseAbility(actor);
        
        GameManager.Instance.RunCoroutine(ChargeCoroutine(actor));
    }

    public IEnumerator ChargeCoroutine(Actor_Base actor)
    {
        // Play charge sound.

        float originalAnimationSpeed = actor.ActorAnimator.speed;
        AnimationClip animation = actor.ActorAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;

        float newAnimationSpeed = animation.length / this.AbilityData.WeaponStats.MaxChargeTime;

        actor.ActorAnimator.speed = newAnimationSpeed;

        actor.ActorAnimator.SetTrigger("Charge");

        yield return new WaitForSeconds(animation.length);

        actor.ActorAnimator.speed = originalAnimationSpeed;
    }
}
