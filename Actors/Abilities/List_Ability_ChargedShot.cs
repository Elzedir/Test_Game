using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class List_Ability_ChargedShot : List_Ability
{
    public List_Ability_ChargedShot
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
        float maxChargeTime
        )
    {
        this.AbilityData.AbilityStats.AbilityName = name;
        this.AbilityData.AbilityStats.AbilityDescription = description;
        this.AbilityData.AbilityStats.AbilityIcon = icon;
        this.AbilityData.AbilityStats.AbilitySpecialisation = specialisation;
        this.AbilityData.AbilityStats.AbilityMaxLevel = maxLevel;
        this.AbilityData.AbilityStats.AbilityDamagePerAbilityLevel = damagePerAbilityLevel;
        this.AbilityData.AbilityStats.RequiredType = requiredType;

        this.AbilityData.CombatStats.Health = itemHealth;
        this.AbilityData.CombatStats.Mana = itemMana;
        this.AbilityData.CombatStats.Stamina = itemStamina;
        this.AbilityData.CombatStats.PushRecovery = itemPushRecovery;
        this.AbilityData.CombatStats.AttackDamage = itemAttackDamage;
        this.AbilityData.CombatStats.AttackSpeed = itemAttackSpeed;
        this.AbilityData.CombatStats.AttackSwingTime = itemAttackSwingTime;
        this.AbilityData.CombatStats.AttackRange = itemAttackRange;
        this.AbilityData.CombatStats.AttackPushForce = itemAttackPushForce;
        this.AbilityData.CombatStats.AttackCooldown = itemAttackCooldown;
        this.AbilityData.CombatStats.PhysicalDefence = itemPhysicalDefence;
        this.AbilityData.CombatStats.MagicalDefence = itemMagicalDefence;
        this.AbilityData.CombatStats.DodgeCooldown = itemDodgeCooldown;

        this.AbilityData.WeaponStats.WeaponType = weaponType;
        this.AbilityData.WeaponStats.WeaponClass = weaponClass;
        this.AbilityData.WeaponStats.MaxChargeTime = maxChargeTime;
    }

    public override void UseAbility(Actor_Base actor)
    {
        base.UseAbility(actor);

        // Play a certain animation.
        GameManager.Instance.RunCoroutine(ChargedShotCoroutine(actor));
        // Shoot a specific arrow
        // Play a specific sound
    }

    public IEnumerator ChargedShotCoroutine(Actor_Base actor)
    {
        float originalAnimationSpeed = actor.ActorAnimator.speed;
        AnimationClip animation = actor.ActorAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;

        float newAnimationSpeed = animation.length / this.AbilityData.WeaponStats.MaxChargeTime;

        actor.ActorAnimator.speed = newAnimationSpeed;

        actor.ActorAnimator.SetTrigger("TempAbilityUse");

        yield return new WaitForSeconds(this.AbilityData.WeaponStats.MaxChargeTime);

        actor.ActorAnimator.speed = originalAnimationSpeed;

        actor.GetComponentInChildren<Weapon_Bow>().WeaponAttack(Ability.ChargedShot);

    }
}
