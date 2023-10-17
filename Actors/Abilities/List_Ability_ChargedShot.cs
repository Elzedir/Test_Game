using System.Collections;
using UnityEngine;

public class List_Ability_ChargedShot : List_Ability
{
    public List_Ability_ChargedShot(AbilityData abilityData)
    {
        this.AbilityData = abilityData;
    }

    public override void UseAbility(Actor_Base actor)
    {
        base.UseAbility(actor);

        // Play a certain animation. Play charging coroutine.
        GameManager.Instance.RunCoroutine(ChargedShotCoroutine(actor));
        // Play a specific sound
    }

    public IEnumerator ChargedShotCoroutine(Actor_Base actor)
    {
        // Play charged shot sound.

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
