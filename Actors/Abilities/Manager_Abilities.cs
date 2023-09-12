using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Manager_Abilities
{
    public static void ActivateAbility(int abilityIndex, Actor_Base actor)
    {
        List_Ability ability = List_Ability.GetAbility(actor.ActorData.ActorAbilities.AbilityList[abilityIndex]);
        
        if (ability != null && GetRemainingCooldownTime(ability, actor) > 0)
        {
            Debug.Log(GetRemainingCooldownTime(ability, actor));
            return;
        }

        ability.UseAbility(actor);

        actor.ActorData.ActorAbilities.AbilityCooldowns[ability] = Time.time;

    }

    public static float GetRemainingCooldownTime(List_Ability ability, Actor_Base actor)
    {
        float remainingTime = actor.ActorData.ActorAbilities.AbilityCooldowns[ability] + ability.AbilityCooldown - Time.time;
        return Mathf.Max(remainingTime, 0f);
    }
}
