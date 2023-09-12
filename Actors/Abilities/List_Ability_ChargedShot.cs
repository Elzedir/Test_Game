using NUnit.Framework.Constraints;
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
        float cooldown,
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
        this.AbilityCooldown = cooldown;
        this.AbilityIcon = icon;
    }

    public override void UseAbility(Actor_Base actor)
    {
        // Play a certain animation.
        GameManager.Instance.RunCoroutine(ChargedShotCoroutine(actor));
        // Shoot a specific arrow
        // Play a specific sound
    }

    public IEnumerator ChargedShotCoroutine(Actor_Base actor)
    {
        float originalAnimationSpeed = actor.ActorAnimator.speed;
        AnimationClip animation = actor.ActorAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;

        float newAnimationSpeed = animation.length / this.AbilityChargeTime;

        actor.ActorAnimator.speed = newAnimationSpeed;

        actor.ActorAnimator.SetTrigger("TempAbilityUse");

        yield return new WaitForSeconds(this.AbilityChargeTime);

        actor.ActorAnimator.speed = originalAnimationSpeed;
    }
}
