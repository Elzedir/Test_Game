using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Equipment_Manager;

public class Manager_Abilities : MonoBehaviour
{
    [SerializeField] public List<List_Ability> UnlockedAbilities = new(); // change this to active abilities and then create a new list of unlocked abilities afterwards.
    public Dictionary<List_Ability, float> AbilityCooldowns = new();

    private Actor_Base _actor;
    
    public void Start()
    {
        _actor = GetComponent<Actor_Base>();
    }

    public void ActivateAbility(int abilityIndex)
    {
        UnlockedAbilities[abilityIndex].UseAbility();
    }

    public void ImportAbilitiesFromScriptableObject(Actor_Data_SO actorData)
    {
        UnlockedAbilities.Clear();
        foreach (Ability ability in actorData.ActorAbilities)
        {
            List_Ability.UnlockAbility(UnlockedAbilities, ability);
        }
    }
}
